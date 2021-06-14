using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace Projeto_Max_Inf_Review
{
    class ConfigBaseDeDados //Teste, teste
    {
        private readonly FormDbConfig formDbconfig;
        private static ConfigBaseDeDados config;

        public string InstanciaSQL;
        public string NomeBD;
        public string Utilizador;
        public string Password;
        private SqlConnection sqlConnection;
        private DataTable table;
        private SqlDataAdapter adapter;

        public string DataSource;
        public string AtachDBname;
        public bool IntegredSecurity;
        public string ConnectTimeOut;

        


        
        public static ConfigBaseDeDados Config
        {
            get
            { 

                if(config == null)
                {
                    config = new ConfigBaseDeDados();
                }
                return config;
             }
        }

        private ConfigBaseDeDados()
        {
            formDbconfig = new FormDbConfig();
        }

        public void TestConexao()
        {
            try
            {
                LoadConfig();
                var conexao = GetConnectionString();
                sqlConnection = new SqlConnection(conexao);
                sqlConnection.Open();

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("ERRO no carregamento do Banco de Dados!" + e.Message);
 
                
            }
        }
        
        internal void Configurar()
        {
            if(formDbconfig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TestConexao();
            }
        }

        private string GetConnectionString()
        {
            string baseconnection = string.Empty;
            if (string.IsNullOrEmpty(InstanciaSQL))
            {
                baseconnection = $"Data Source{ DataSource}; AtachDBName={AtachDBname}";
            }
            else
            {
                baseconnection = $"Server={InstanciaSQL}; DataBase={NomeBD}";
            }

            if (IntegredSecurity)
            {
                return $"{baseconnection}; Integretade Security = {IntegredSecurity.ToString()}; Connect Timeout={ConnectTimeOut};";
            } 
            else
            {
                return $"{baseconnection}; User Id = {Utilizador}; Password = {Password}";
            }
        }

        private void LoadConfig()
        {
            NomeBD = ConfigurationManager.AppSettings.Get("NomeBD");
            InstanciaSQL = ConfigurationManager.AppSettings.Get("InstanciaSQL");
            Utilizador = ConfigurationManager.AppSettings.Get("Utilizador");
            Password = ConfigurationManager.AppSettings.Get("Password");

            DataSource = ConfigurationManager.AppSettings.Get("DataSource");
            AtachDBname = ConfigurationManager.AppSettings.Get("AtachDBname");
            IntegredSecurity = ConfigurationManager.AppSettings.Get("IntegredSecurity") == null ? false : bool.Parse(ConfigurationManager.AppSettings.Get("IntegaredSecurity"));
            ConnectTimeOut = ConfigurationManager.AppSettings.Get("ConnectTimeOut");



        }

        public void SaveConfig()
        {
            var configFile = ConfigurationManager.OpenConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings["InstanciaSQL"].Value = InstanciaSQL;
            settings["NomeDB"].Value = NomeBD;
            settings["Utilizador"].Value = Utilizador;
            settings["Password"].Value = Password;

            settings["DataSource"].Value = DataSource;
            settings["AttachDbFilename"].Value = AttachDbFilename;
            settings["IntegratedSecurity"].Value = IntegratedSecurity.ToString();
            settings["ConnectTimeOut"].Value = ConnectTimeOut;

            configFile.Save(ConfigurationSaveMode.Modfied);
            ConfigurationManager.RefreshSection(configFile.AppSettings.Section.Information.Name);
        }

        internal DataTable ObterDados() //teste
        {
            SqlCommand command = new SqlCommand("select * from artigos", sqlConnection);
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;

            table = new DataTable();
            adapter.Fill(table);

            return table;


        }

        internal void SalvarDados(DataGridViewRow row)
        {
            try
            {
                var stamp = row.Cells["STAMP"].Value;
                var custo = row.Cells["preco_custo"].Value == DBNull.Value ? 0 : row.Cells["preco_custo"].Value;
                var venda = row.Cells["preco_venda"].Value == DBNull.Value ? 0 : row.Cells["preco_venda"].Value;

                string cmd;

                if(stamp == DBNull.Value)
                {
                    cmd = $"insert into artigos(stamp, ref , design, preco_custo, preco_venda) values ({Guid.NewGuid().ToString().Substring(0, 24)}','ref','design',{custo},{venda})";
                }
                else
                {
                    cmd = $"update artigos set preco_custo = {custo}, preco_venda = {venda}, where STAMP = '{stamp}'";
                }

                SqlCommand command = new SqlCommand(cmd, sqlConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Erro ao alterar a tabela{e.Message}");
                
            }

            
        }

        internal void RemoverRegistro(DataGridViewRow row)
        {
            try
            {
                SqlCommand command = new SqlCommand($"delete from artigos whrere STAMP = '{row.Cells["STAMP"].Value}'", sqlConnection);
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {

                MessageBox.Show($"Erro ao exluir registro!{e.Message}");
            }
        }



    }
}
