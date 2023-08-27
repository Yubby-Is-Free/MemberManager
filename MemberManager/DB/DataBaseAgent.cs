using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace MemberManager.DB
{
    public class DataBaseAgent
    {
        private DataTable Member = new DataTable();

        public DataBaseAgent()
        {
            InitDataTable();
            SetDataFromCSV();
        }

        private void InitDataTable()
        {
            Member.Columns.Add("Name", typeof(string));
            Member.Columns.Add("Age", typeof(int));
            Member.Columns.Add("Phone", typeof(string));
            Member.Columns.Add("IsFix", typeof(int)); //삭제 불가능한 데이터 구분 컬럼
        }

        //.Net5.0에 sqlConnection 클래스가 없어 CSV를 이용하여 파일 데이터베이스 형식으로 구축.
        private void SetDataFromCSV()
        {
            string[] values = File.ReadAllLines(Environment.CurrentDirectory + "\\Data\\member_data.csv", Encoding.Default);

            for(int rowIndex = 1; rowIndex < values.Length; rowIndex++)
            {
                string[] value = values[rowIndex].Split(',');

                DataRow row = Member.NewRow();

                row["Name"] = value[0];
                row["Age"] = value[1];
                row["Phone"] = value[2];
                row["IsFix"] = value[3];

                Member.Rows.Add(row);
            }
        }

        public List<DataRow> GetMember(string Name, string Phone)
        {
            List<DataRow> rows = new List<DataRow>();

            if(Name == "" && Phone == "")
            {
                rows = Member.Select().ToList();
            }
            else if(Name != "" && Phone == "")
            {
                rows = Member.Select($"Name = '{Name}'").ToList();
            }
            else if(Name == "" && Phone != "")
            {
                rows = Member.Select($"Phone = '{Phone}'").ToList();
            }
            else
            {
                rows = Member.Select($"Name = '{Name}' AND Phone = '{Phone}'").ToList();
            }

            return rows;
        }

        public bool DeleteMember(string Name, string Phone)
        {
            List<DataRow> rows = Member.Select($"Name = '{Name}' AND Phone = '{Phone}'").ToList();

            //기존 10명의 멤버데이터 삭제 방지를 위한 값 확인
            if (int.Parse(rows[0]["IsFix"].ToString()) == 1)
            {
                return false;
            }
            else
            {
                try
                {
                    Member.Rows.Remove(rows[0]);

                    StringBuilder csvText = new StringBuilder();
                    csvText.Append("Name,Age,Phone,IsFix\r\n");

                    foreach(DataRow r in Member.Rows)
                    {
                        csvText.Append(r["Name"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["Age"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["Phone"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["IsFix"].ToString());
                        csvText.Append("\r\n");
                    }

                    File.WriteAllText(Environment.CurrentDirectory + "\\Data\\member_data.csv", csvText.ToString(), Encoding.Default);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        
        public bool AddMember(string Name, int Age, string Phone)
        {
            List<DataRow> rows = Member.Select($"Name = '{Name}' AND Phone = '{Phone}'").ToList();

            if(rows.Count > 0)
            {
                return false;
            }
            else
            {
                try
                {
                    DataRow row = Member.NewRow();

                    row["Name"] = Name;
                    row["Age"] = Age;
                    row["Phone"] = Phone;
                    row["IsFix"] = 0;

                    Member.Rows.Add(row);

                    StringBuilder csvText = new StringBuilder();
                    csvText.Append("Name,Age,Phone,IsFix\r\n");

                    foreach (DataRow r in Member.Rows)
                    {
                        csvText.Append(r["Name"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["Age"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["Phone"].ToString());
                        csvText.Append(",");
                        csvText.Append(r["IsFix"].ToString());
                        csvText.Append("\r\n");
                    }

                    File.WriteAllText(Environment.CurrentDirectory + "\\Data\\member_data.csv", csvText.ToString(), Encoding.Default);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

    }
}
