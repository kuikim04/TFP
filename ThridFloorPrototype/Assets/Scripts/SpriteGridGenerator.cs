using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGridGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] roofPrefab;
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private float _spriteScale = 5f;
    [SerializeField] private float _verticalOverlap = 0.2f;
    [SerializeField] private float _horizontalSpacing = 0.1f;

    [Header("")]
    [SerializeField] private Sprite[] _spritesRegion1_1;
    [SerializeField] private Sprite[] _spritesRegion1_2;

    [SerializeField] private Sprite[] _spritesRegion2_1;
    [SerializeField] private Sprite[] _spritesRegion2_2;

    [SerializeField] private Sprite[] _spritesRegion3_1;
    [SerializeField] private Sprite[] _spritesRegion3_2;

    [SerializeField] private Sprite[] _spritesRegion4_1;
    [SerializeField] private Sprite[] _spritesRegion4_2;

    private Dictionary<Vector2, Tile> _tiles;

    [SerializeField] private Transform camTrans;


    [SerializeField] private Image bg;
    [SerializeField] private Sprite[] bgRegion1;
    [SerializeField] private Sprite[] bgRegion2;
    [SerializeField] private Sprite[] bgRegion3;
    [SerializeField] private Sprite[] bgRegion4;

    void Start()
    {
        LoadLevelData();
        GenerateGrid();
        SetBackgroundByRegion();
    }

    private void SetBackgroundByRegion()
    {
        int currentRegion = DataCenter.Instance.GetPlayerData().region;

        switch (currentRegion)
        {
            case 1:
                bg.sprite = GetRandomBackground(bgRegion1);
                break;
            case 2:
                bg.sprite = GetRandomBackground(bgRegion2);
                break;
            case 3:
                bg.sprite = GetRandomBackground(bgRegion3);
                break;
            case 4:
                bg.sprite = GetRandomBackground(bgRegion4);
                break;
            default:
                Debug.LogWarning("Unknown region: " + currentRegion);
                break;
        }
    }

    private Sprite GetRandomBackground(Sprite[] backgrounds)
    {
        if (backgrounds.Length == 0)
        {
            Debug.LogError("Background array is empty.");
            return null;
        }
        int randomIndex = Random.Range(0, backgrounds.Length);
        return backgrounds[randomIndex];
    }

    private void LoadLevelData()
    {
        var csvFile = DataCenter.Instance.GetCsvFile();
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        foreach (var line in lines)
        {
            string[] columns = line.Split(',');

            if (columns.Length > 0 && int.TryParse(columns[0], out int region) &&
                region == DataCenter.Instance.Region &&
                int.TryParse(columns[1], out int stage) &&
                stage == DataCenter.Instance.Stage &&
                int.TryParse(columns[2], out int level))
            {
                switch (level)
                {
                    case 1:
                    case 2:
                    case 3:
                        _width = 3;
                        _height = level + 1;
                        camTrans.position = new Vector3(2.5f, 5, -10);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        _width = 3;
                        _height = level + 1;
                        camTrans.position = new Vector3(2.5f, 5, -10);
                        break;
                    case 7:
                        _width = 3;
                        _height = level + 1;
                        camTrans.position = new Vector3(2.5f, 7.5f, -10);
                        break;
                    default:
                        Debug.LogWarning($"Level {level} is not handled.");
                        break;
                }
                break;
            }
        }
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();

        Sprite[] sprites1 = null;
        Sprite[] sprites2 = null;

        switch (DataCenter.Instance.Region)
        {
            case 1:
                sprites1 = _spritesRegion1_1;
                sprites2 = _spritesRegion1_2;
                break;
            case 2:
                sprites1 = _spritesRegion2_1;
                sprites2 = _spritesRegion2_2;
                break;
            case 3:
                sprites1 = _spritesRegion3_1;
                sprites2 = _spritesRegion3_2;
                break;
            case 4:
                sprites1 = _spritesRegion4_1;
                sprites2 = _spritesRegion4_2;
                break;
            default:
                Debug.LogError($"Unsupported region: {DataCenter.Instance.Region}");
                return;
        }

        if (sprites1 == null || sprites1.Length == 0 || sprites2 == null || sprites2.Length == 0)
        {
            Debug.LogError("Please assign enough sprites for both sprite arrays.");
            return;
        }

        if (DataCenter.Instance.Region != 1 && DataCenter.Instance.Region != 2)
        {
            sprites2 = ShuffleSprites(sprites2);
        }

        Vector3 currentPosition = Vector3.zero;

        var sampleSpriteRenderer = _tilePrefab.GetComponent<SpriteRenderer>();
        sampleSpriteRenderer.sprite = sprites1[0];
        sampleSpriteRenderer.transform.localScale = new Vector3(_spriteScale, _spriteScale, 1);
        float spriteWidth = sampleSpriteRenderer.sprite.bounds.size.x * _spriteScale;
        float spriteHeight = sampleSpriteRenderer.sprite.bounds.size.y * _spriteScale;

        for (int y = _height - 1; y >= 0; y--)
        {
            for (int x = 0; x < _width; x++)
            {
                GenerateTile(sprites1, currentPosition, (_height - y - 1) * 2, x, y);
                GenerateTile(sprites2, currentPosition, (_height - y) * 2 - 1, x, y);

                currentPosition.x += spriteWidth * (1 - _horizontalSpacing);
            }

            currentPosition.x = 0;
            currentPosition.y += spriteHeight * (1 - _verticalOverlap);

        }

        float roofOffset = spriteHeight * (1 - _verticalOverlap) + 1.0f;
        Vector3 roofPosition = new Vector3((_width - 1) * spriteWidth / 2, (_height - 1) *
            spriteHeight * (1 - _verticalOverlap) + roofOffset, 0);

        switch (DataCenter.Instance.Region)
        {
            case 1:
                Instantiate(roofPrefab[0], roofPosition, Quaternion.identity);
                break;
            case 2:
                Instantiate(roofPrefab[1], roofPosition, Quaternion.identity);
                break;
            case 3:
                Instantiate(roofPrefab[2], roofPosition, Quaternion.identity);
                break;
            case 4:
                Instantiate(roofPrefab[3], roofPosition, Quaternion.identity);
                break;
        }
    }


    void GenerateTile(Sprite[] sprites, Vector3 position, int sortingOrder, int x, int y)
    {
        var spawnedTile = Instantiate(_tilePrefab, position, Quaternion.identity);
        spawnedTile.name = $"Tile {x} {y}";

        var spriteRenderer = spawnedTile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[(x + y * _width) % sprites.Length];
            spawnedTile.transform.localScale = new Vector3(_spriteScale, _spriteScale, 1);
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }

        _tiles[new Vector2(x, y)] = spawnedTile;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
    private Sprite[] ShuffleSprites(Sprite[] sprites)
    {
        Sprite[] shuffledSprites = (Sprite[])sprites.Clone();
        for (int i = shuffledSprites.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = shuffledSprites[i];
            shuffledSprites[i] = shuffledSprites[j];
            shuffledSprites[j] = temp;
        }
        return shuffledSprites;
    }

}
