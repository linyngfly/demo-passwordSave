import { createApp, Application, Session, connector } from "mydog"

import * as log4js from "log4js";
log4js.configure({
    appenders: { mydog: { type: 'file', filename: 'mydog.log' }, console: { type: "console" } },
    categories: { default: { appenders: ['mydog', "console"], level: 'all' } }
});
export let infolog = log4js.getLogger("mydog");

import mysqlClient from "./app/domain/mysql";
import { mysqlConfig } from "./app/domain/dbConfig";
import { loginHttpStart } from "./app/domain/login";


let app = createApp();

app.setEncodeDecodeConfig({ "msgDecode": msgDecode });

app.setConnectorConfig({ "connector": connector.connectorTcp, "heartbeat": 5 });

app.configure("connector", function () {
    app.set("mysql", new mysqlClient(mysqlConfig));
});

app.configure("gate", function () {
    app.set("mysql", new mysqlClient(mysqlConfig));
    loginHttpStart(app);
});


app.onLog(function (level: string, info: string) {
    infolog.error(level, info);

});

app.start();


process.on("uncaughtException", function (err: any) {
    infolog.error("uncaughtException", err);
})

function msgDecode(cmdId: number, msgBuf: Buffer): any {
    infolog.error(app.routeConfig[cmdId]);
    return JSON.parse(msgBuf as any);
}
