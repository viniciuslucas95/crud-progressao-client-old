using crud_progressao.Models;
using System.Collections.ObjectModel;

namespace crud_progressao.Scripts {
    internal static class ListHelper {
        internal static ObservableCollection<Student> FilterOwingOnly(ObservableCollection<Student> students) {
            ObservableCollection<Student> filteredStudents = new ();

            foreach (Student student in students)
                if (student.IsOwing) filteredStudents.Add(student);

            return filteredStudents;
        }
    }
}
