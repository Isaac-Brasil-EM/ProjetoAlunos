
using Domain;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;


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

        private int _nextId = 1;


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

        public override void Add(Aluno aluno)
        {

            string queryValidKey = "Select matricula from TBALUNO";
            string query = "insert into TBALUNO values('" + aluno.Matricula + "','" + aluno.Nome + "','" + aluno.Cpf + "'," + (int)aluno.Sexo  + ",'" + aluno.Nascimento.ToString("yyyyMMdd") + "')";
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
                    transaction.Commit();
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
                    //transaction.Commit();

                }
            }
        }
        
        public override void Update(Aluno aluno)
        {
            string query = "update TBALUNO set Nome= '" + aluno.Nome + "', Matricula='" + aluno.Matricula + "', Cpf='" + aluno.Cpf + "', Nascimento ='" + aluno.Nascimento.ToString("yyyyMMdd") + "', Sexo ='" + (int)aluno.Sexo + "' where Matricula ='" + aluno.Matricula + "' ";
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
                    transaction.Commit();

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
                                int codigo = reader.GetInt32(0);
                                string nome = reader.GetString(1);
                                string cpf = reader.GetString(2);
                                EnumeradorSexo sexo = (EnumeradorSexo) reader.GetInt32(3);
                                string nascimento = reader.GetInt32(4).ToString();


                                //trocar o nascimento aqui para um parse de int para date
                                list.Add(new Aluno { Matricula = codigo, Nome = nome, Cpf = cpf, Sexo = sexo, Nascimento = DateTime.ParseExact(nascimento,
                                    "yyyyMMdd",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None)
                                });
                            }
                        }
                    }
                }
            }
            return list;
        }

        public override IEnumerable<Aluno> Get(Expression<Func<Aluno, bool>> predicate)
        {
            List<Aluno> list = new List<Aluno>();
            string query = "select * from TBALUNO WHERE TBALUNO.MATRICULA";
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
                        
                        foreach (DataRow dr in dt.Rows)
                        {
                            list.Add(new Aluno { Matricula = Convert.ToInt32(dr[0]), Nome = Convert.ToString(dr[1]), Cpf = Convert.ToString(dr[2]), Nascimento = DateTime.Parse(dr[3].ToString()), Sexo = (EnumeradorSexo)Convert.ToInt32(dr[4]) });
                        }
                    }

                }
            }
            //return list.FirstOrDefault();[
            return GetAll().Where(predicate.Compile()); 
        }
    }
}

