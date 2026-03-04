using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    private Tile[,] _grid;
    private bool _canDrawConnection = false;

    private List<Tile> _connections = new List<Tile>();
    private Tile _connectionTile; // Tile hiện tại đang nối (đuôi rắn)

    private int _dimensionX = 0;
    private int _dimensionY = 0;
    private int _solved = 0;
    private Dictionary<int, int> _amountToSolve = new Dictionary<int, int>();

    void Start()
    {
        // 1. Setup Grid Dimensions
        _dimensionY = transform.childCount; // Số hàng (Rows)
        if (_dimensionY > 0)
            _dimensionX = transform.GetChild(0).childCount; // Số cột (Cols)

        _grid = new Tile[_dimensionX, _dimensionY];

        // 2. Loop qua các hàng (Rows)
        for (int y = 0; y < _dimensionY; y++)
        {
            var row = transform.GetChild(y);
            row.name = "" + y;

            // Loop qua các cột (Cols) trong hàng
            for (int x = 0; x < _dimensionX; x++)
            {
                var tile = row.GetChild(x).GetComponent<Tile>();
                tile.name = "" + x;

                // Set tọa độ vào Tile luôn để tiện truy xuất
                tile.gridX = x;
                tile.gridY = y;

                // Đăng ký sự kiện
                tile.onSelected.AddListener(onTileSelected);
                tile.onHover.AddListener(onTileHover); // [MỚI] Nghe sự kiện lướt chuột

                _CollectAmountToSolveFromTile(tile);
                _grid[x, y] = tile;
            }
        }
        SetGameStatus(_solved, _amountToSolve.Count);
    }

    // [LOGIC MỚI] Xử lý khi chuột lướt vào một Tile (Thay cho Update)
    void onTileHover(Tile hoverTile)
    {
        // Nếu không đang trong trạng thái vẽ thì bỏ qua
        if (!_canDrawConnection) return;

        // Logic cũ của bạn: Kiểm tra các điều kiện
        Tile firstTile = _connections[0];

        // Cùng màu bắt đầu nhưng khác ô (tránh nối lại chính đầu rắn)
        bool isDifferentActiveTile = hoverTile.cid > 0 && hoverTile.cid != firstTile.cid;

        if (hoverTile.isHighlighted || hoverTile.isSolved || isDifferentActiveTile) return;

        // Kiểm tra vị trí: Hover Tile vs Connection Tile (đuôi hiện tại)
        if (hoverTile == _connectionTile) return; // Vẫn đang ở ô cũ

        // Tính khoảng cách Grid
        var deltaX = Mathf.Abs(_connectionTile.gridX - hoverTile.gridX);
        var deltaY = Mathf.Abs(_connectionTile.gridY - hoverTile.gridY);

        bool isShiftNotOnNext = deltaX > 1 || deltaY > 1; // Nhảy cóc
        bool isShiftDiagonal = (deltaX > 0 && deltaY > 0); // Đi chéo

        if (isShiftNotOnNext || isShiftDiagonal) return;

        // --- HỢP LỆ -> VẼ NỐI ---
        hoverTile.Highlight();
        hoverTile.SetConnectionColor(_connectionTile.ConnectionColor);

        // Vẽ ống nối từ ô cũ sang ô mới
        _connectionTile.ConnectionToSide(
            hoverTile.gridY > _connectionTile.gridY, // Top (Y tăng) - Lưu ý: UI Grid có thể Y ngược tùy setup, check lại nếu bị ngược
            hoverTile.gridX > _connectionTile.gridX, // Right
            hoverTile.gridY < _connectionTile.gridY, // Bottom
            hoverTile.gridX < _connectionTile.gridX  // Left
        );

        // Cập nhật đuôi rắn
        _connectionTile = hoverTile;
        _connections.Add(_connectionTile);

        // Check Win Condition
        if (_CheckIfTilesMatch(hoverTile, firstTile))
        {
            // Hoàn thành 1 đường
            _connections.ForEach((tile) => tile.isSolved = true);
            _canDrawConnection = false;

            if (_amountToSolve.ContainsKey(firstTile.cid))
                _amountToSolve.Remove(firstTile.cid);

            SetGameStatus(++_solved, _amountToSolve.Count + _solved);

            if (_amountToSolve.Keys.Count == 0)
            {
                Debug.Log("GAME COMPLETE");
                // Có thể gọi DesktopManager để hiện thông báo thắng
            }
        }
    }

    // --- CÁC HÀM PHỤ TRỢ (Giữ nguyên hoặc chỉnh nhẹ) ---

    void _CollectAmountToSolveFromTile(Tile tile)
    {
        if (tile.cid > Tile.UNPLAYABLE_INDEX)
        {
            if (_amountToSolve.ContainsKey(tile.cid))
                _amountToSolve[tile.cid] += 1;
            else _amountToSolve[tile.cid] = 1;
        }
    }

    bool _CheckIfTilesMatch(Tile tile, Tile another)
    {
        return tile.cid > 0 && another.cid == tile.cid;
    }

    void onTileSelected(Tile tile)
    {
        if (tile.isSelected)
        {
            // Bắt đầu vẽ
            _connectionTile = tile;
            _connections = new List<Tile>();
            _connections.Add(_connectionTile);
            _canDrawConnection = true;
            _connectionTile.Highlight();
        }
        else
        {
            // Nhả chuột
            bool isFirstTileInConnection = _connectionTile == tile;
            if (isFirstTileInConnection)
            {
                tile.HightlightReset();
            }
            else if (!_CheckIfTilesMatch(_connectionTile, tile))
            {
                // Nhả chuột giữa đường -> Reset
                _ResetConnections();
            }
            _canDrawConnection = false;
        }
    }

    public void onRestart()
    {
        foreach (var tile in _grid)
        {
            if (tile == null) continue;
            tile.ResetConnection();
            tile.HightlightReset();
            // Reset logic đếm...
        }
        // Logic restart đếm số cần check lại tùy game logic của bạn
        // Ở đây tôi giữ simple
    }

    void _ResetConnections()
    {
        _connections.ForEach((tile) =>
        {
            tile.ResetConnection();
            tile.HightlightReset();
        });
        _connections.Clear();
    }

    void SetGameStatus(int solved, int from)
    {
        // Tìm txtStatus trong con của Field hoặc cha
        // Lưu ý: Find("txtStatus") tìm toàn scene, có thể chậm hoặc nhầm
        // Tốt nhất nên serialize field này
        var statusGO = GameObject.Find("txtStatus");
        if (statusGO != null)
        {
            var txt = statusGO.GetComponent<Text>();
            if (txt) txt.text = $"Solve: {solved} from {from}";
        }
    }
}