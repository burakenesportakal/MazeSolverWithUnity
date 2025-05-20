using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // Labirentin genişliği (sütun sayısı)
    int width = 21;

    // Labirentin yüksekliği (satır sayısı)
    int height = 21;

    [Header("Maze Prefabs")]
    // Duvar olarak kullanılacak prefab
    public GameObject wallPrefab;

    // Yürünebilir yol için kullanılacak prefab
    public GameObject pathPrefab;

    // Başlangıç noktasına konacak bayrak prefabı
    public GameObject startFlagPrefab;

    // Çıkış noktasına konacak bayrak prefabı
    public GameObject goalFlagPrefab;

    // Maze objelerinin sahnede toplanacağı boş GameObject referansı
    public Transform mazeParent;

    // Labirentin 2 boyutlu mantıksal matrisi (0 = yol, 1 = duvar)
    public int[,] maze;
    // Sahnedeki maze hücrelerinin referanslarını tutar
    public GameObject[,] tiles;

    [Header("Camera Settings")]
    // Labirent görüntüsü için ana kamera referansı
    public Camera mainCamera;

    // Kamera pozisyon ve dönüş ayarlarını tutacak yapı
    [System.Serializable]
    public struct CameraSettings
    {
        public Vector3 position;    // Kamera konumu
        public Vector3 eulerAngles; // Kamera rotasyonu (Euler açıları)
    }

    // Küçük labirent için kamera ayarları
    public CameraSettings smallCamSettings;

    // Orta labirent için kamera ayarları
    public CameraSettings mediumCamSettings;

    // Büyük labirent için kamera ayarları
    public CameraSettings bigCamSettings;

    // Verilen kamera ayarlarını ana kameraya uygular
    void SetCameraPosition(CameraSettings settings)
    {
        mainCamera.transform.position = settings.position;                      // Kamera pozisyonu ayarlanır
        mainCamera.transform.rotation = Quaternion.Euler(settings.eulerAngles); // Kamera rotasyonu ayarlanır
    }

    // Başlat butonuna basıldığında çağrılır, labirenti oluşturur ve sahnede gösterir
    public void StartButton()
    {
        ClearMaze();    // Önceden varsa labirenti temizler
        GenerateMaze(); // Labirent matrisi oluşturulur
        SpawnMaze();    // Labirent objeleri sahnede oluşturulur
        FindObjectOfType<MazeSolver>().Initialize(maze, tiles); // MazeSolver başlatılır ve maze bilgisi verilir
    }

    // Uygulamayı kapatır
    public void QuitButton()
    {
        Application.Quit();
    }

    // Küçük boyut seçilince çağrılır
    public void SmallSizeButton()
    {
        width = 21;                      // Labirent genişliği ve yüksekliği ayarlanır
        height = 21;
        ClearMaze();                    // Mevcut labirent sahneden temizlenir
        SetCameraPosition(smallCamSettings); // Küçük boyut için kamera pozisyonu ayarlanır
    }

    // Orta boyut seçilince çağrılır
    public void MediumSizeButton()
    {
        width = 41;
        height = 41;
        ClearMaze();
        SetCameraPosition(mediumCamSettings);
    }

    // Büyük boyut seçilince çağrılır
    public void BigSizeButton()
    {
        width = 61;
        height = 61;
        ClearMaze();
        SetCameraPosition(bigCamSettings);
    }

    // Sahnedeki tüm labirent objelerini ve bayrakları temizler
    public void ClearMaze()
    {
        if (mazeParent == null)
        {
            Debug.LogWarning("mazeParent reference not assigned!"); // mazeParent atanmamışsa uyarı verir
            return;
        }

        // mazeParent içindeki tüm child objeleri (labirent parçaları) siler
        for (int i = mazeParent.childCount - 1; i >= 0; i--)
        {
            Destroy(mazeParent.GetChild(i).gameObject);
        }   
    }

    // Labirent matrisini oluşturur (başta tüm hücreler duvar)
    void GenerateMaze()
    {
        maze = new int[width, height]; // Labirent matrisini yeni boyutta oluşturur

        // Tüm hücreleri duvar olarak işaretler (1)
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        Carve(1, 1); // Labirentte geçilebilir yollar açmaya başlar
    }

    // Labirenti açmak için derinlik öncelikli rekürsif backtracking yöntemiyle yol açar
    void Carve(int x, int y)
    {
        maze[x, y] = 0; // Mevcut hücreyi yol yap (0)

        // Dört yönü temsil eden vektörler
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // Yönlerin sırasını karıştırır, böylece rastgele labirent oluşur
        dirs = Shuffle(dirs);

        foreach (var dir in dirs)
        {
            int nx = x + dir.x * 2; // İki hücre ilerideki x konumu
            int ny = y + dir.y * 2; // İki hücre ilerideki y konumu

            // Eğer iki hücre ilerisi maze sınırları içinde ve hala duvarsa (1) yol aç
            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 1)
            {
                maze[x + dir.x, y + dir.y] = 0; // Aradaki hücreyi de yol yap
                Carve(nx, ny);                  // İki hücre ilerideki noktada tekrar kazma işlemi
            }
        }
    }

    // Yönlerin sırasını karıştırır
    Vector2Int[] Shuffle(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int r = Random.Range(i, array.Length); // Rastgele bir index seç
            (array[i], array[r]) = (array[r], array[i]); // Elemanları takas et
        }
        return array; // Karışmış dizi döner
    }

    // Maze matrisine göre sahnede labirent objelerini oluşturur
    void SpawnMaze()
    {
        tiles = new GameObject[width, height]; // Labirentin her hücresi için referans dizisi oluşturur
        float spacing = 1f; // Hücreler arası mesafe: 1 birim

        // Matris boyutunda tüm hücreleri dolaşır
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Hücre değeri 1 ise duvar, 0 ise yol seçilir
                GameObject prefab = (maze[x, y] == 1) ? wallPrefab : pathPrefab;

                // Hücre dünya pozisyonu hesaplanır x ve y ekseninde
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);

                // Prefab instantiate edilir ve mazeParent altına yerleştirilir
                tiles[x, y] = Instantiate(prefab, pos, Quaternion.identity, mazeParent);
            }
        }

        // Başlangıç bayrağının pozisyonu (labirentin sol üst köşesine yakın)
        Vector3 startPos = new Vector3(1 * spacing, 0.5f, 1 * spacing);

        // Bitiş bayrağının pozisyonu (labirentin sağ alt köşesine yakın)
        Vector3 goalPos = new Vector3((width - 2) * spacing, 0.5f, (height - 2) * spacing);

        // Başlangıç ve bitiş bayrakları instantiate edilir ve mazeParent altına yerleştirilir
        Instantiate(startFlagPrefab, startPos, Quaternion.identity, mazeParent);
        Instantiate(goalFlagPrefab, goalPos, Quaternion.identity, mazeParent);
    }
}
