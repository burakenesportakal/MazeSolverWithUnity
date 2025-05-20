using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSolver : MonoBehaviour
{
    // Maze'in matris olarak tutulduğu değişken (0 = yol, 1 = duvar)
    private int[,] maze;

    // Her maze hücresi için sahnedeki GameObject referansları (labirent kareleri)
    private GameObject[,] tiles;

    // DFS algoritmasında kullanılan stack yapısı
    private Stack<Vector2Int> stack;

    // Hangi hücrelerin ziyaret edildiğini tutan boolean dizi
    private bool[,] visited;

    // Her hücrenin hangi hücreden geldiğini tutan sözlük (backtracking için)
    private Dictionary<Vector2Int, Vector2Int> cameFrom;

    // Her adım arasında bekleme süresi (güzel görünüm için animasyon gibi düşünülebilir)
    public float stepDelay = 0.01f;

    // Başlangıç ve hedef noktaları (labirent koordinatları)
    private Vector2Int start;
    private Vector2Int goal;

    // MazeSolver başlatılırken çağrılır, labirent verisini ve tile referanslarını alır
    public void Initialize(int[,] mazeData, GameObject[,] tileRefs)
    {
        maze = mazeData; // Labirent matrisi
        tiles = tileRefs; // Sahnedeki kareler
        stack = new Stack<Vector2Int>(); // Yeni boş stack oluştur
        visited = new bool[maze.GetLength(0), maze.GetLength(1)]; // Ziyaret dizisini oluştur
        cameFrom = new Dictionary<Vector2Int, Vector2Int>(); // Backtracking için boş sözlük

        // Başlangıç hücresi (genellikle sol üstten biraz içeride)
        start = new Vector2Int(1, 1);
        // Hedef hücresi (genellikle sağ altta biraz içeride)
        goal = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);

        // Çözüm algoritmasını başlat (Coroutine ile animasyonlu)
        StartCoroutine(Solve());
    }

    // Labirent çözüm algoritması olarak DFS kullanır
    IEnumerator Solve()
    {
        stack.Push(start); // Başlangıç hücresini yığına ekle

        // Stack boşalana kadar devam et
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop(); // Yığından bir hücre çıkar

            // Hücre geçerli değilse veya daha önce ziyaret edildiyse atla
            if (!IsValid(current) || visited[current.x, current.y])
                continue;

            visited[current.x, current.y] = true; // Hücreyi ziyaret edilmiş olarak işaretle

            // Hücreyi mavi renge boya (geçilen yol animasyonu için)
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;

            // Biraz bekle ki animasyon gözlemlenebilsin
            yield return new WaitForSeconds(stepDelay);

            // Eğer hedefe ulaştıysak
            if (current == goal)
            {
                // Çözüm yolunu geri izleyerek göster
                StartCoroutine(TracePath(current));
                yield break; // Algoritmayı durdur
            }

            // Komşu hücreleri (yukarı, aşağı, sol, sağ) sırayla kontrol et
            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir; // Komşu hücre koordinatları

                // Komşu geçerli ve ziyaret edilmemişse
                if (IsValid(next) && !visited[next.x, next.y])
                {
                    stack.Push(next); // Komşuyu yığına ekle

                    // Eğer komşu için henüz yol bilgisi yoksa kaydet
                    if (!cameFrom.ContainsKey(next))
                        cameFrom[next] = current; // Komşunun geldiği hücreyi kaydet
                }
            }
        }
    }

    // Çözüm yolunu geri izleyerek yeşil renkle gösteren coroutine
    IEnumerator TracePath(Vector2Int current)
    {
        // Başlangıçtan hedefe giden yolu geriye doğru takip et
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current]; // Önceki hücreye geç
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.green; // Hücreyi yeşil boya
            yield return new WaitForSeconds(stepDelay / 2); // Daha hızlı ilerle
        }
    }

    // Verilen pozisyonun labirent içinde olup olmadığını ve yol olup olmadığını kontrol eder
    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < maze.GetLength(0) &&  // X sınırı
               pos.y >= 0 && pos.y < maze.GetLength(1) &&  // Y sınırı
               maze[pos.x, pos.y] == 0;                      // Yol olup olmadığı (0 = yol, 1 = duvar)
    }
}
