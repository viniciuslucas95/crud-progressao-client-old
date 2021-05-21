namespace crud_progressao_user_register.Models {
    public struct User {
        internal string Id { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal bool Privilege { get; set; }

        internal string PrivilegeString {
            get {
                if (Privilege) return "Sim";

                return "Não";
            }
        }
    }
}
