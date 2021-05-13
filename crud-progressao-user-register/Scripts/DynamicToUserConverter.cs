using crud_progressao_library.DataTypes;
using crud_progressao_user_register.Models;

namespace crud_progressao_user_register.Scripts {
    public class DynamicToUserConverter : IDynamicConverter<User> {
        public User Convert(dynamic data) {
            return new User() {
                Id = data._id,
                Username = data.username,
                Password = data.password,
                Privilege = data.privilege
            };
        }
    }
}
