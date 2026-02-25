using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; private set; }

        public string Nome { get; private set; } = string.Empty;

        public string Senha { get; private set; } = string.Empty;

        private User() { }

        public User(string nome, string senha)
        {
            Nome = nome;
            Senha = senha;
        }
    }
}
