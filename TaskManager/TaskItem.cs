using System;

namespace TaskManager
{
    // Статус задачи
    public enum TaskStatus
    {
        Новая,
        ВРаботе,
        Выполнена
    }

    // Приоритет задачи
    public enum TaskPriority
    {
        Низкий,
        Средний,
        Высокий
    }

    // Класс одной задачи
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }

        public TaskItem()
        {
            CreatedDate = DateTime.Now;
            Status = TaskStatus.Новая;
            Priority = TaskPriority.Средний;
        }

        // Для отображения в списке
        public override string ToString()
        {
            return $"[{Priority}] {Title} — {Status}";
        }
    }
}
