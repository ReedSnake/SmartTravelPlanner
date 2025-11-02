public class Logger<T> {
    private List<T> log = new List<T>();
    public void Add(T text) {
        log.Add(text);
    }
    public void Flush(string name) {
        foreach (T l in log) {
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("yyyy-MM-dd HH:mm");
            string lineWithTime = $"{timestamp} | {l}";
            File.AppendAllText(name, lineWithTime + "\n");
        }
        log.Clear();
    }
}