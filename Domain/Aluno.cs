namespace Domain
{
    public class Aluno : IEntidade
    {
        public int Matricula { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime Nascimento { get; set; }
        public EnumeradorSexo Sexo { get; set; }



        public override string ToString()
        {

            return "oi";
        }

        public override bool Equals(object obj)
        {
            var item = obj as Aluno;
            if (item == null)
            {
                return false;
            }
            return true;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }

}

