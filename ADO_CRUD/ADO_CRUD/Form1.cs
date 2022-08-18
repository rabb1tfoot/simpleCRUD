using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;
using static ADO_CRUD.Form1;

namespace ADO_CRUD
{
    public partial class Form1 : Form
    {
        private MySqlConnection conn;
        BindingList<LanguageDto> languageList;
        public long g_Id = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            languageList = new BindingList<LanguageDto>();
            
            string strConn = "Server = localhost; Database = sakila; Uid = root; Pwd = 1;";
            conn = new MySqlConnection(strConn);
            conn.Open();
            SelectAll(languageList);

            bindingSource1.DataSource = languageList;
            dataGridView1.DataSource = bindingSource1;
        }

        public class LanguageDto : INotifyPropertyChanged
        {
            private long language_id;
            private String name;
            private String last_update;

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public long Id
            {
                get
                {
                    return this.language_id;
                }
            }
            public String Name
            {
                get
                {
                    return this.name;
                }
            }
            public String Date
            {
                get
                {
                    return this.last_update;
                }
            }

            public LanguageDto(long id, String inputName, String date)
            {
                language_id = id;
                name = inputName;
                last_update = date;
            }
        }

        private void DBInsert(String name)
        {

            string sql = String.Format("INSERT INTO language (language_id, name, last_update) VALUES ({0}, \"{1}\", '{2}')", ++g_Id, name, GetCurrentTimeStamp());

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private void DBUpdate(string id, string name)
        {
            if(!int.TryParse(id, out int value))
            {
                return;
            }
            if(value > g_Id)
            {
                return;
            }
            string sql = String.Format("UPDATE language SET name=\"{0}\", last_update='{1}' WHERE language_id='{2}'", name, GetCurrentTimeStamp(), id);

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

        }
        private void DBSearch(string name, BindingList<LanguageDto> LanguageDtos)
        {
            string sql = String.Format("SELECT * FROM language WHERE name like'%{0}%'", name);
            LanguageDtos.Clear();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine("{0}: {1} : {2}", rdr["language_id"], rdr["name"], rdr["last_update"]);
                LanguageDtos.Add(new LanguageDto((long)Convert.ToDouble(rdr["language_id"].ToString()), rdr["name"].ToString(), rdr["last_update"].ToString()));
                g_Id++;
            }
            rdr.Close();
        }

        private void DBDelete(String id)
        {
            if (!int.TryParse(id, out int value))
            {
                return;
            }

            string sql = String.Format("DELETE FROM language WHERE language_id = '{0}'", id);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        private void SelectAll(BindingList<LanguageDto> LanguageDtos)
        {
            g_Id = 0;
            LanguageDtos.Clear();
            string sql = "SELECT * FROM language";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();         
            while (rdr.Read())
            {
                Console.WriteLine("{0}: {1} : {2}", rdr["language_id"], rdr["name"], rdr["last_update"]);
                LanguageDtos.Add(new LanguageDto((long)Convert.ToDouble(rdr["language_id"].ToString()), rdr["name"].ToString(), rdr["last_update"].ToString())); 
                g_Id++;
            }
            rdr.Close();
        }
        public String GetCurrentTimeStamp()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString("yyyy-MM-dd HH:mm:ss");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }

        private void btn_read_Click(object sender, EventArgs e)
        {
            SelectAll(languageList);
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            DBInsert(tb_name.Text);
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            DBDelete(tb_Id.Text);
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            DBUpdate(tb_Id.Text, tb_name.Text);
        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            DBSearch(tb_name.Text, languageList);
        }

    }
}