namespace crud_progressao_users.Models {
    public struct User {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Privilege { get; set; }

        public string PrivilegeString {
            get {
                if (Privilege) return "Sim";

                return "Não";
            }
        }
    }
}
