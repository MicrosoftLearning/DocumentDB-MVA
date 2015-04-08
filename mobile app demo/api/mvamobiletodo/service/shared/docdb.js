var config = require('mobileservice-config'),
    DocumentDBClient = require('documentdb').DocumentClient;

var host = config.appSettings.endpoint,
    authKey = config.appSettings.authKey,
    collectionLink = config.appSettings.collectionLink;

var client = new DocumentDBClient(host, { masterKey: authKey });

var Utils = {
    fetch: function (query, callback) {        
        client.queryDocuments(collectionLink, query).toArray(function (err, docs) {
            if (err) {
                callback(err);
            } else {
                callback(null, docs);
            }
        })
    },

    get: function (id, callback) {
        var querySpec = {
            query: "SELECT * FROM r WHERE r.id=@id",
            parameters: [
                { name: "@id", value: id }
            ]
        };

        client.queryDocuments(collectionLink, querySpec).toArray(function (err, results) {
            if (err) {
                callback(err);
            } else {
                var doc = results[0];
                callback(null, doc);
            }
        });
    },

    create: function(doc, callback) {
        client.createDocument(collectionLink, doc, function(err, created) {
            if (err) {
                callback(err);
            } else {
                callback(null, created.id);
            }
        })
    },

    replace: function (docLink, doc, callback) {
        client.replaceDocument(docLink, doc, function (err, result) {
            if (err) {
                callback(err);
            } else {
                callback(null, result);
            }
        });
    },

    del: function (docLink, callback) {
        client.deleteDocument(docLink, function (err, result) {
            if (err) {
                callback(err);
            } else {
                callback(null, result);
            }
        });
    }
}

module.exports = Utils;