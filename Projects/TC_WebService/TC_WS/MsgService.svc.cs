using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace TC_WS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MsgService" in code, svc and config file together.
    public class MsgService : IMsgService
    {
        public Boolean postMessage(string receiverId, string messageText)
        {
            try
            {
                DataClassesDataContext db = new DataClassesDataContext();
                Message msgobj = new Message();
                msgobj.UserID = receiverId;
                msgobj.MessageText = messageText;
                db.Messages.InsertOnSubmit(msgobj);
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataTable getMyMessages(string receiverId)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            var qres = from Message message in db.Messages where message.UserID == receiverId select message;

            DataTable returntable = new DataTable();
            DataColumn idColumn = new DataColumn();
            idColumn.DataType = System.Type.GetType("System.Int32");
            idColumn.ColumnName = "id";
            returntable.Columns.Add(idColumn);

            DataColumn aNameColumn = new DataColumn();
            aNameColumn.DataType = System.Type.GetType("System.String");
            aNameColumn.ColumnName = "userid";
            returntable.Columns.Add(aNameColumn);

            DataColumn bNameColumn = new DataColumn();
            bNameColumn.DataType = System.Type.GetType("System.String");
            bNameColumn.ColumnName = "message";
            returntable.Columns.Add(bNameColumn);

            foreach (Message qitem in qres){
                DataRow row = returntable.NewRow();
                row["id"] = qitem.ID;
                row["userid"] = qitem.UserID;
                row["message"] = qitem.MessageText;
                returntable.Rows.Add(row);
            }

            return returntable;
        }
    }
}
