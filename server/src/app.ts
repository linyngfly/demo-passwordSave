import { createApp, Application, Session } from "mydog"
import mysqlClient from "./app/domain/mysql";
import { mysqlConfig } from "./app/domain/dbConfig";
import { loginHttpStart } from "./app/domain/login";

let app = createApp();

app.set_encodeDecodeConfig({ "decode": decode });

app.configure("connector", function () {
    //"ws" for cocos creator,  "net" for unity
    app.set("connectorConfig", { connector: "net", heartbeat: 10 });
    app.set("mysql", new mysqlClient(mysqlConfig));
});

app.configure("gate", function () {
    app.set("mysql", new mysqlClient(mysqlConfig));
    loginHttpStart(app);
});


app.onLog(function (filename: string, level: string, info: string) {
    // console.log(level, filename, info);
});

app.start();


process.on("uncaughtException", function (err: any) {
    console.log(err)
})

function decode(cmdId: number, msgBuf: Buffer): any {
    console.log(app.routeConfig[cmdId]);
    // console.log(JSON.parse(msgBuf as any));
    return JSON.parse(msgBuf as any);
}
