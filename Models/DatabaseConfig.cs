namespace Domain.Models
{
    public class PostgresDatabaseConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Engine { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string DatabaseName { get; set; }

        public override string ToString()
        {
            return $"User ID={Username};Password={Password};Host={Host};Port={Port};Database={DatabaseName};Pooling=true;";
        }
    }
}

