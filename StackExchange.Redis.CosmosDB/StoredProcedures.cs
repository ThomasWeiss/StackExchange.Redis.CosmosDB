namespace StackExchange.Redis.CosmosDB
{
    class StoredProcedures
    {
        public const string runStringCommand = @"function runStringCommand(command, args) {
    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();

    switch (command) {
        case 'INCR':
        case 'INCRBYFLOAT':
        case 'DECR':
        case 'DECRBYFLOAT':
            runIncrDecr(command, args);
            break;
        case 'GETSET':
            runGetSet(args);
            break;
        default:
            response.setBody({ success: false, errorMessage: `Unsupported command: ${command}` });
    }

    function runIncrDecr(command, args) {
        var exists = true;
        collection.readDocument(
            `${collection.getAltLink()}/docs/${args.key}`,
            function (err, doc) {
                if (err) {
                    if (err.number == 404) {
                        doc = { id: args.key, type: 'string', val: '0' };
                        exists = false;
                    }
                    else { throw err; }
                }
                if (doc.type != 'string') {
                    response.setBody({ success: false, errorMessage: 'value is not a string' });
                    return;
                }
                var value = command.endsWith('FLOAT') ? parseFloat(doc.val) : parseInt(doc.val);
                if (!isNumber(value)) {
                    response.setBody({ success: false, errorMessage: 'value is not an integer or out of range' });
                    return;
                }
                if (command.startsWith('INCR')) value += args.val; else value -= args.val;
                doc.val = value.toString();
                if (exists) {
                    collection.replaceDocument(
                        doc._self,
                        doc,
                        function (err) {
                            if (err) throw err;
                            response.setBody({ success: true, result: value });
                        });
                } else {
                    collection.createDocument(
                        collection.getSelfLink(),
                        doc,
                        function (err) {
                            if (err) throw err;
                            response.setBody({ success: true, result: value });
                        });
                }
            });
    }

    function runGetSet(args) {
        collection.readDocument(
            `${collection.getAltLink()}/docs/${args.key}`,
            function (err, doc) {
                if (err) {
                    if (err.number == 404) {
                        collection.createDocument(
                            collection.getSelfLink(),
                            { id: args.key, type: 'string', val: args.val },
                            function (err) {
                                if (err) throw err;
                                response.setBody({ success: true, result: null });
                            });
                    } else { throw err; }
                } else {
                    var currentVal = doc.val;
                    doc.val = args.val;
                    collection.replaceDocument(
                        doc._self,
                        doc,
                        function (err) {
                            if (err) throw err;
                            response.setBody({ success: true, result: currentVal });
                        });
                }
            });
    }

    function isNumber(v) { return typeof v === 'number' && isFinite(v); }
}";
    }
}
