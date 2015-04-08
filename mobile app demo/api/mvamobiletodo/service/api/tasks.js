var docdb = require('../shared/docdb.js');

var handleError = function (err) {
    console.log(err);
    throw err;
}

exports.register = function (app) {
    app.get('*',
        function (request, response) {
            docdb.fetch("SELECT * FROM tasks t WHERE NOT t.complete", function (err, results) {
                if (err) handleError(err);

                response.setHeader('Content-Type', 'application/json');
                response.send(statusCodes.OK, JSON.stringify(results));
            })
        });

    app.get('/:id',
        function (request, response) {
            var id = request.params.id;
            
            docdb.get(id, function (err, doc) {
                if (err) handleError(err);

                response.setHeader('Content-Type', 'application/json');
                response.send(statusCodes.OK, JSON.stringify(doc));
            })
        });

    app.post('*',
        function (request, response) {
            var item = {
                desc: request.body.desc,
                complete: request.body.complete || false
            };

            docdb.create(item, function (err, docId) {
                if (err) handleError(err);

                response.setHeader('Content-Type', 'application/json');
                response.send(statusCodes.OK, JSON.stringify(docId));
            });
        });

    app.put('*', 
        function (request, response) {
            var id = request.body.id;

            docdb.get(id, function (err, found) {
                if (err) handleError(err);

                var item = {
                    id: id,
                    desc: request.body.desc,
                    complete: request.body.complete || false
                };

                docdb.replace(found._self, item, function (err, updated) {
                    if (err) handleError(err);

                    response.setHeader('Content-Type', 'application/json');
                    response.send(statusCodes.OK, JSON.stringify(updated));
                });
            });
        });

    app.patch('/:id',
        function (request, response) {
            var id = request.params.id;

            docdb.get(id, function (err, found) {
                if (err) handleError(err);

                found.complete = true;
                docdb.replace(found._self, found, function (err, updated) {
                    if (err) handleError(err);

                    response.setHeader('Content-Type', 'application/json');
                    response.send(statusCodes.OK, JSON.stringify(updated));
                });
            });
        });

    app.delete('/:id',
        function (request, response) {
            var id = request.params.id;

            docdb.get(id, function (err, found) {
                if (err) handleError(err);

                docdb.del(found._self, function (err, deleted) {
                    if (err) handleError(err);

                    response.setHeader('Content-Type', 'application/json');
                    response.send(statusCodes.OK, JSON.stringify(deleted));
                });
            });
        });
}