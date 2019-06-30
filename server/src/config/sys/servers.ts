export default {
    "development": {
        "gate": [
            { "id": "gate-server-1", "host": "127.0.0.1", "port": 5005 }
        ],
        "connector": [
            { "id": "connector-server-1", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5006, "frontend": true, "clientPort": 4001 },
            { "id": "connector-server-2", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5007, "frontend": true, "clientPort": 4002 }
        ]

    },
    "production": {
        "gate": [
            { "id": "gate-server-1", "host": "127.0.0.1", "port": 5005, }
        ],
        "connector": [
            { "id": "connector-server-1", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5006, "frontend": true, "clientPort": 4001 },
            { "id": "connector-server-2", "host": "127.0.0.1", "clientHost": "127.0.0.1", "port": 5007, "frontend": true, "clientPort": 4002 }
        ]
    }
}