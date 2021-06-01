using crud_progressao_students.Models;
using System.Collections.ObjectModel;

namespace crud_progressao_students.Scripts {
    internal static class ListHelper {
        internal static ObservableCollection<Student> FilterOwingOnly(ObservableCollection<Student> students) {
            ObservableCollection<Student> filteredStudents = new ();

            foreach (Student student in students)
                if (student.IsOwing) filteredStudents.Add(student);

            return filteredStudents;
        }

        internal static ObservableCollection<Student> RemoveDeactivated(ObservableCollection<Student> students) {
            ObservableCollection<Student> filteredStudents = new();

            foreach (Student student in students)
                if (!student.IsDeactivated) filteredStudents.Add(student);

            return filteredStudents;
        }
    }
}
