using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多设备数据采集与监控.DAL
{
    public class SQLiteHelper
    {
        
        private string _connString = Path.Combine(Application.StartupPath, "Data/mydata.db");

        public SQLiteHelper()
        {
            _connString = $"Data Source={_connString};";
        }
        public int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                   int res = cmd.ExecuteNonQuery();
                    if (res == 1)
                    {
                        return 1;
                    }return 0;
                }
            }
           
        }

        public object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable ExecuteDataTable(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // 将 DataTable 转换为 List<T>
        public List<T> DataTableToList<T>(DataTable dt) where T : new()
        {
            var list = new List<T>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (DataRow row in dt.Rows)
            {
                T item = new T();
                foreach (var prop in props)
                {
                    if (dt.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        prop.SetValue(item, Convert.ChangeType(row[prop.Name], prop.PropertyType));
                    }
                }
                list.Add(item);
            }

            return list;
        }

        // 直接执行查询并转为 List<T>
        public List<T> Query<T>(string sql, params SQLiteParameter[] parameters) where T : new()
        {
            var dt = ExecuteDataTable(sql, parameters);
            return DataTableToList<T>(dt);
        }


        
    }
}
        
    
