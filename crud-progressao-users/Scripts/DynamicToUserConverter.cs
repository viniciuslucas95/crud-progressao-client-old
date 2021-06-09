using crud_progressao_library.DataTypes;
using crud_progressao_users.Models;

namespace crud_progressao_users.Scripts {
    public class DynamicToUserConverter : IDynamicConverter<User> {
        public User Convert(dynamic data) {
            return new User() {
                Id = data._id,
                Username = data.username,
                Privilege = data.privilege
            };
        }
    }
}
