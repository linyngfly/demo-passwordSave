export default {
    "development": {
        "gate": [
            { "id": "gate-server-1", "host": "127.0.0.1", "port": 5005, "alone": true }
        ],
        "connector": [
            { "id": "connector-server-1", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5006, "frontend": true },
            { "id": "connector-server-2", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5007, "frontend": true }
        ]
    },
    "production": {
        "gate": [
            { "id": "gate-server-1", "host": "127.0.0.1", "port": 5005, "alone": true }
        ],
        "connector": [
            { "id": "connector-server-1", "host": "127.0.0.1", "clientHost": "47.105.51.196", "port": 5006, "frontend": true },
            { "id": "connector-server-2", "host": "127.0.0.1", "clientHost": "47.105.51.196", "port": 5007, "frontend": true }
        ]
    }
}