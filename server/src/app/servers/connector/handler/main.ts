import { Application, Session } from "mydog";
import mysqlClient from "../../../domain/mysql";

export default function (app: Application) {
    return new Handler(app);
}

class Handler {
    app: Application;
    mysql: mysqlClient;
    constructor(app: Application) {
        this.app = app;
        this.mysql = app.get("mysql") as mysqlClient;
    }

    // 登入
    entry(msg: any, session: Session, next: Function) {
        let self = this;
        this.app.rpc.toServer("gate-server-1").gate.main.isTokenOK(msg.uid, msg.token, function (err: any, ok: boolean) {
            if (err || !ok) {
                next({ "code": -1 });
                return;
            }
            self.mysql.query("select data from account where uid = ?", [msg.uid], function (err: any, res: any) {
                if (err || res.length === 0) {
                    next({ "code": -1 });
                    return;
                }
                let data: string = res[0].data || "";

                // 断开当前其他的所有该用户连接
                self.app.sendMsgByUid("onKicked", null, [msg.uid]);
                self.app.closeClient(msg.uid);
                let cons = self.app.getServersByType("connector");
                for (let i = 0; i < cons.length; i++) {
                    if (cons[i].id !== self.app.serverId) {
                        self.app.rpc.toServer(cons[i].id).connector.main.kickUser(msg.uid);
                    }
                }

                // 绑定该连接
                session.bind(msg.uid);
                next({ "code": 0, "data": data });
            });
        });
    }

    // 更新数据
    updateData(msg: any, session: Session, next: Function) {
        if (!session.uid) {
            session.close();
            return;
        }
        this.mysql.query("update account set data = ? where uid = ?", [msg.data, session.uid], function (err: any) {
            if (err) {
                next({ "code": -1 });
                return;
            }
            next({ "code": 0 });
        });
    }

    // 修改密码
    changePassword(msg: any, session: Session, next: Function) {
        if (!session.uid) {
            session.close();
            return;
        }
        this.mysql.query("update account set password = ? , data = ? where uid = ?", [msg.password, msg.data, session.uid], function (err: any) {
            if (err) {
                next({ "code": -1 });
                return;
            }
            next({ "code": 0 });
        });
    }

}