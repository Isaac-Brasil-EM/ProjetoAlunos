﻿
using Domain;
using FirebirdSql.Data.FirebirdClient;
using Repository;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;


namespace Repository
{
    public class RepositorioAluno : RepositorioAbstrato<Aluno>
    {

        private static string User = "SYSDBA";
        private static string Password = "masterkey";
        private static string Database = "localhost:D:\\DBALUNO.fdb";
        private static int Port = 3054;
        private static string Dialect = "3";
        private static string Charset = FbCharset.None.ToString();

        FbConnectionStringBuilder conn = new FbConnectionStringBuilder()
        {
            Port = Port,
            Password = Password,
            Database = Database,
            UserID = User,
            Charset = Charset,

        };
        public Aluno GetByMatricula(int matricula)
        {
            return Get(aluno => aluno.Matricula == matricula).FirstOrDefault();
        }
        public IEnumerable<Aluno> GetByContendoNoNome(string parteDoNome)
        {
            return Get(aluno => aluno.Nome.ToLower().Contains(parteDoNome)).ToList();
        }

        /*aluno.Matricula = id;
        var dt = CONNECTAR.RetornoTabela($"SELECT * FROM TBALUNO WHERE TBALUNO.MATRICULA = {id}");
        List<Aluno> Listar_alunos = Aluno.ListarTabela(dt);
        return Listar_alunos.FirstOrDefault();
        return aluno;*/

        public override void Add(Aluno aluno)
        {
            string query = "insert into TBALUNO values('" + aluno.Matricula + "','" + aluno.Nome + "','" + aluno.Cpf + "'," + aluno.Nascimento + ",'" + (int)aluno.Sexo + "')";
            using (var con = new FbConnection(conn.ToString()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    using (var command = new FbCommand(query, con, transaction))
                    {
                        command.Connection = con;
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        int i = command.ExecuteNonQuery();
                    }
                }
            }
        }

        public override void Remove(Aluno aluno)
        {
            string query = "delete from TBALUNO where MATRICULA='" + aluno.Matricula + "'";
            using (var con = new FbConnection(conn.ToString()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    using (var command = new FbCommand(query, con, transaction))
                    {
                        command.Connection = con;
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        int i = command.ExecuteNonQuery();
                    }

                }
            }
        }

        public override void Update(Aluno aluno)
        {
            string query = "update TBALUNO set Nome= '" + aluno.Nome + "', Matricula='" + aluno.Matricula + "', Cpf='" + aluno.Cpf + "', Nascimento ='" + aluno.Nascimento.Date + "', Sexo ='" + (int)aluno.Sexo + "' where Matricula ='" + id + "' ";
            using (var con = new FbConnection(conn.ToString()))
            {
                con.Open();


                using (var transaction = con.BeginTransaction())
                {

                    using (var command = new FbCommand(query, con, transaction))
                    {
                        command.Connection = con;

                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        int i = command.ExecuteNonQuery();
                    }

                }
            }
        }

        public override IEnumerable<Aluno> GetAll()
        {
            List<Aluno> list = new List<Aluno>();
            string query = "Select * from TBALUNO ";
            using (var con = new FbConnection(conn.ToString()))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {

                    using (var command = new FbCommand(query, con, transaction))
                    {
                        command.Connection = con;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int codigo = reader.GetInt32(1);
                                string nome = reader.GetString(2);
                                string cpf = reader.GetString(3);
                                EnumeradorSexo sexo = (EnumeradorSexo) reader.GetInt32(4);
                                DateTime nascimento = reader.GetDateTime(5);

                                list.Add(new Aluno { Matricula = codigo, Nome = nome, Cpf = cpf, Sexo = sexo, Nascimento = nascimento});
                            }
                        }
                    }

                }
            }
            return list;
        }

        public override T Get(int Id)
        {
            List<Aluno> list = new List<Aluno>();
            string query = $"select * from TBALUNO WHERE TBALUNO.MATRICULA = {id}";
            using (var con = new FbConnection(conn.ToString()))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {

                    using (var command = new FbCommand(query, con, transaction))
                    {
                        command.Connection = con;
                        FbDataAdapter fbda = new(command);
                        DataTable dt = new DataTable();
                        fbda.Fill(dt);
                        foreach (DataRow dr in dt.Rows)
                        {
                            list.Add(new Aluno { Matricula = Convert.ToInt32(dr[0]), Nome = Convert.ToString(dr[1]), Cpf = Convert.ToString(dr[2]), Nascimento = DateTime.Parse(dr[3].ToString()), Sexo = (EnumeradorSexo)Convert.ToInt32(dr[4]) });
                        }
                    }

                }
            }
            return list.FirstOrDefault();
        }
    }
}
