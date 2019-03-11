import mysqlClient from "./mysql";
import * as http from "http"
import * as querystring from "querystring"
import { Application } from "mydog";



let PORT = 5001;
let app: Application;
let mysql: mysqlClient;
let nowConHost: string = "127.0.0.1";
let nowConPort: number = 3000;

let msgHandler: normalObj = {};
msgHandler.login = function (msg: any, next: Function) {
    mysql.query("select uid, password from account where username = ?", [msg.username], function (err: any, res: any) {
        if (err) {
            next({ "code": -1 });
            return;
        }
        if (res.length === 0) {
            next({ "code": 1, "msg": "用户或密码错误" });
            return;
        }
        if (res[0].password !== msg.password) {
            next({ "code": 2, "msg": "用户或密码错误" });
            return;
        }
        let token = Math.floor(Math.random() * 10000000);
        mysql.query("update account set token = ? where uid = ?", [token, res[0].uid], function (err: any) {
            if (err) {
                next({ "code": -1 });
                return;
            }
            let result = { "code": 0, "uid": res[0].uid, "host": nowConHost, "port": nowConPort, "token": token };
            next(result);
        });
    });
};

msgHandler.register = function (msg: any, next: Function) {
    mysql.query("select uid from account where username = ?", [msg.username], function (err: any, res: any) {
        if (err) {
            next({ "code": -1 });
            return;
        }
        if (res.length !== 0) {
            next({ "code": 1, "msg": "username already exists" });
            return;
        }
        mysql.query("insert into account(username, password, regtime) values(?, ?, ?)", [msg.username, msg.password, new Date()], function (err: any, res: any) {
            if (err) {
                next({ "code": -1 });
                return;
            }
            next({ "code": 0 });
        });
    });
};


function callback(response: http.ServerResponse) {
    return function (data: any) {
        if (data === undefined) {
            data = null;
        }
        response.end(JSON.stringify(data));
    }
}


export function loginHttpStart(_app: Application) {
    app = _app;
    mysql = app.get("mysql");
    let server = http.createServer(function (request: http.IncomingMessage, response: http.ServerResponse) {
        if (request.method !== "POST") {
            return;
        }
        let msg = "";
        request.on("data", function (chuck) {
            msg += chuck;
        });
        request.on("end", function () {
            let body = querystring.parse(msg) as any;
            console.log(body);
            if (body && body.method && msgHandler[body.method]) {
                msgHandler[body.method](body, callback(response));
            }
        });
    });
    server.listen(PORT, function () {
        console.log("--- login server (http)  running at port: " + PORT + ".");
    });
    server.on("error", function (err) {
        console.log("--- login server error::", err.message);
    });

    setInterval(getUserNum, 5 * 1000);
};


function getUserNum() {
    let cons = app.getServersByType("connector");
    for (let i = 0; i < cons.length; i++) {
        rpcGetNum(cons[i].id);
    }
    setMinUserIp();
}

function rpcGetNum(serverId: string) {
    try {
        app.rpc.toServer(serverId).connector.main.getUserNum(function (err: any, num: number) {
            if (err) {
                return;
            }
            let server = app.serversIdMap[serverId];
            if (server) {
                server.userNum = num;
            }
        });
    } catch (e) {

    }
}


function setMinUserIp() {
    let cons = app.getServersByType("connector");
    if (cons.length === 0) {
        return;
    }
    let minNum = cons[0].userNum || 0;
    let minIndex = 0;
    for (let i = 1; i < cons.length; i++) {
        if (cons[i].userNum < minNum) {
            minNum = cons[i].userNum;
            minIndex = i;
        }
    }
    nowConHost = cons[minIndex].clientHost;
    nowConPort = cons[minIndex].port;
}