namespace RSNiniChanger
{
    struct ServerObject : IEquatable<ServerObject>
    {
        public string Server;
        public bool IsMarked;
        public ServerObject(string server, bool isMarked)
        {
            Server = server;
            IsMarked = isMarked;
        }

        public bool Equals(ServerObject obj)
        {
            return (this.Server == obj.Server);
        }
        public override int GetHashCode()
        {
            string hashStr = Server;
            return hashStr.GetHashCode();
        }
    }
}
