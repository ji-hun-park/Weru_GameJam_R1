using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ScreenUI : MonoBehaviour
{
    public enum BrushType { Brush, Pencil, Pen, Crayon }
    public enum DrawMode { Brush, Line, Rectangle, Circle, Select }
    
    public RawImage targetImage; // 그림을 그릴 UI Image (RawImage 사용 권장)
    public int textureWidth = 512; // 캔버스 너비
    public int textureHeight = 512; // 캔버스 높이
    public Color drawColor = Color.black; // 브러쉬 색상
    public float brushSize = 5f; // 브러시 크기
    private Stack<Color[]> undoStack = new Stack<Color[]>(); // Undo 히스토리
    private Stack<Color[]> redoStack = new Stack<Color[]>(); // Redo 히스토리
    
    public Button blackButton;  // UI 버튼 (검정색)
    public Button navyButton;  // UI 버튼 (군청색)
    public Button redButton;    // UI 버튼 (빨간색)
    public Button blueButton;   // UI 버튼 (파란색)
    public Button greenButton;  // UI 버튼 (녹색)
    public Button brownButton;  // UI 버튼 (갈색)
    public Button orangeButton; // UI 버튼 (주황색)
    public Button yellowButton; // UI 버튼 (노란색)
    public Button skyblueButton;// UI 버튼 (하늘색)
    public Button purpleButton; // UI 버튼 (보라색)
    public Slider brushSizeSlider; // 브러시 크기 조절 슬라이더
    public Button brushButton; // 기본 브러쉬
    public Button lineButton;  // 직선 그리기
    public Button rectButton;  // 사각형 그리기
    public Button circleButton;// 원형 그리기
    public Button BrushButton; // 기본 브러쉬
    public Button pencilButton;// 연필 질감
    public Button penButton;   // 만년필 질감
    public Button crayonButton;// 크레파스 질감
    public Button selectButton; // 영역선택 버튼
    public Button undoButton; // 되돌리기 버튼
    public Button redoButton; // 원래대로 버튼
    public Button eraseButton; // 지우개 버튼
    public Button clearButton; // 선택 영역 지우기 버튼
    
    private Texture2D drawTexture;  // 기본 질감
    private Texture2D pencilTexture;// 연필 질감
    private Texture2D penTexture;   // 만년필 질감
    private Texture2D crayonTexture;// 크레파스 질감
    private Texture2D tmpT2D;
    private Sprite iconSprite;
    private RectTransform rectTransform;
    private BrushType currentBrush = BrushType.Brush;
    private DrawMode currentMode = DrawMode.Brush;
    private Vector2 selectionStart;  // 선택 시작점
    private Vector2 selectionEnd;    // 선택 끝점
    //private bool isSelecting = false; // 영역 선택 여부
    private Rect selectionRect; // 선택된 영역 정보
    public Color selectionColor = new Color(0, 1, 0, 0.3f); // 선택 영역 시각화 색상


    private Vector2 startPoint; // 시작 점
    private bool isDrawing = false;

    void Start()
    {
        InitCanvas();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentMode != DrawMode.Select)
        {
            SaveUndoState(); // 그리기 전 현재 상태 저장
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                startPoint = new Vector2(x, y);
                isDrawing = true;

                if (currentMode == DrawMode.Brush)
                {
                    DrawBrush((int)x, (int)y);
                }
            }
        }
        else if (Input.GetMouseButton(0) && isDrawing && currentMode == DrawMode.Brush)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                DrawBrush((int)x, (int)y);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDrawing && currentMode != DrawMode.Select)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;

                Vector2 endPoint = new Vector2(x, y);

                if (currentMode == DrawMode.Line) DrawLine(startPoint, endPoint);
                else if (currentMode == DrawMode.Rectangle) DrawRectangle(startPoint, endPoint);
                else if (currentMode == DrawMode.Circle) DrawCircle(startPoint, endPoint);

                isDrawing = false;
            }
        }

        if (currentMode == DrawMode.Select)
        {
            HandleMouseInput();
        }

        // 저장 단축키 감지
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveTexture();
        }
        
        // 불러오기 단축키 감지
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadTexture();
        }
        
        // 전송 단축키 감지
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnClickSendButton();
        }
        
        // 뒤로가기 단축키 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickReturnButton();
        }
        
        // Undo 실행
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }

        // Redo 실행
        if (Input.GetKeyDown(KeyCode.X))
        {
            Redo();
        }
        
        // 캔버스 초기화
        if (Input.GetKeyDown(KeyCode.N))
        {
            InitDrawPaper();
        }
    }
    
    public static Color ConvertToUnityColor(int r, int g, int b, int a = 255)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public void InitDrawPaper()
    {
        // 브러쉬 색 초기화
        drawColor = Color.black;
        
        //브러쉬 초기화
        SetDrawMode(DrawMode.Brush);
        
        // 새 텍스처 생성
        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                drawTexture.SetPixel(x, y, Color.white); // 초기화
            }
        }
        drawTexture.Apply();
        
        // 텍스처를 RawImage에 연결
        targetImage.texture = drawTexture;
        rectTransform = targetImage.GetComponent<RectTransform>();
        
        Debug.Log("Initialized Draw Paper");
    }
    
    public void InitCanvas()
    {
        // 캔버스 초기화
        InitDrawPaper();
        
        // 버튼 이벤트 연결
        if (blackButton != null) blackButton.onClick.AddListener(() => ChangeColor(Color.black));
        if (navyButton != null) navyButton.onClick.AddListener(() => ChangeColor(ConvertToUnityColor(0,0,128)));
        if (redButton != null) redButton.onClick.AddListener(() => ChangeColor(Color.red));
        if (blueButton != null) blueButton.onClick.AddListener(() => ChangeColor(Color.blue));
        if (greenButton != null) greenButton.onClick.AddListener(() => ChangeColor(Color.green));
        if (brownButton != null) brownButton.onClick.AddListener(() => ChangeColor(ConvertToUnityColor(89,51,29)));
        if (yellowButton != null) yellowButton.onClick.AddListener(() => ChangeColor(Color.yellow));
        if (orangeButton != null) orangeButton.onClick.AddListener(() => ChangeColor(ConvertToUnityColor(255,102,0)));
        if (skyblueButton != null) skyblueButton.onClick.AddListener(() => ChangeColor(ConvertToUnityColor(135,206,235)));
        if (purpleButton != null) purpleButton.onClick.AddListener(() => ChangeColor(ConvertToUnityColor(128,0,128)));
        if (brushButton != null) brushButton.onClick.AddListener(() => SetDrawMode(DrawMode.Brush));
        if (lineButton != null) lineButton.onClick.AddListener(() => SetDrawMode(DrawMode.Line));
        if (rectButton != null) rectButton.onClick.AddListener(() => SetDrawMode(DrawMode.Rectangle));
        if (circleButton != null) circleButton.onClick.AddListener(() => SetDrawMode(DrawMode.Circle));
        if (pencilButton != null) pencilButton.onClick.AddListener(() => SetBrushType(BrushType.Pencil));
        if (penButton != null) penButton.onClick.AddListener(() => SetBrushType(BrushType.Pen));
        if (crayonButton != null) crayonButton.onClick.AddListener(() => SetBrushType(BrushType.Crayon));
        if (undoButton != null) undoButton.onClick.AddListener(() => Undo());
        if (redoButton != null) redoButton.onClick.AddListener(() => Redo());
        if (selectButton != null) selectButton.onClick.AddListener(() => SetDrawMode(DrawMode.Select));
        if (eraseButton != null) eraseButton.onClick.AddListener(() => ChangeColor(Color.white));
        if (clearButton != null) clearButton.onClick.AddListener(() => DeleteArea());
        
        // 슬라이더 이벤트 연결
        if (brushSizeSlider != null)
        {
            brushSizeSlider.minValue = 1f;
            brushSizeSlider.maxValue = 20f;
            brushSizeSlider.value = brushSize;
            brushSizeSlider.onValueChanged.AddListener(ChangeBrushSize);
        }
        
        // 아이콘 세팅
        tmpT2D = Resources.Load<Texture2D>("Icons/Brush");
        iconSprite = Sprite.Create(tmpT2D, new Rect(0, 0, tmpT2D.width, tmpT2D.height), new Vector2(0.5f, 0.5f));
        if (BrushButton != null) BrushButton.gameObject.GetComponent<Image>().sprite = iconSprite;
        tmpT2D = Resources.Load<Texture2D>("Icons/Pencil");
        iconSprite = Sprite.Create(tmpT2D, new Rect(0, 0, tmpT2D.width, tmpT2D.height), new Vector2(0.5f, 0.5f));
        if (pencilButton != null) pencilButton.gameObject.GetComponent<Image>().sprite = iconSprite;
        tmpT2D = Resources.Load<Texture2D>("Icons/Pen");
        iconSprite = Sprite.Create(tmpT2D, new Rect(0, 0, tmpT2D.width, tmpT2D.height), new Vector2(0.5f, 0.5f));
        if (penButton != null) penButton.gameObject.GetComponent<Image>().sprite = iconSprite;
        tmpT2D = Resources.Load<Texture2D>("Icons/Crayon");
        iconSprite = Sprite.Create(tmpT2D, new Rect(0, 0, tmpT2D.width, tmpT2D.height), new Vector2(0.5f, 0.5f));
        if (crayonButton != null) crayonButton.gameObject.GetComponent<Image>().sprite = iconSprite;
        tmpT2D = Resources.Load<Texture2D>("Icons/SelectArea");
        iconSprite = Sprite.Create(tmpT2D, new Rect(20f, 20f, tmpT2D.width-20f, tmpT2D.height-20f), new Vector2(0.5f, 0.5f));
        if (selectButton != null) selectButton.gameObject.GetComponent<Image>().sprite = iconSprite;
        
        // 브러쉬 질감 텍스처들 가져오기
        LoadBrushTextures();
    }

    void DeleteArea()
    {
        Vector2 start = ScreenToTextureCoord(selectionStart);
        Vector2 end = ScreenToTextureCoord(selectionEnd);
        ClearSelectionArea(start, end, brushSize);
    }
    
    void HandleMouseInput()
    {
        // 마우스 드래그로 영역 선택
        if (Input.GetMouseButtonDown(0))
        {
            selectionStart = Input.mousePosition; // 시작 지점
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectionEnd = Input.mousePosition; // 끝 지점
            // 테두리 그리기
            Vector2 start = ScreenToTextureCoord(selectionStart);
            Vector2 end = ScreenToTextureCoord(selectionEnd);
            DrawSelectionBorder(start, end, Color.gray, brushSize); // 두께 n의 회색 테두리
        }

        // 선택 영역 지우기
        if (Input.GetKeyDown(KeyCode.Delete)) // 'Delete' 키로 영역 삭제
        {
            ClearSelectionArea(ScreenToTextureCoord(selectionStart), ScreenToTextureCoord(selectionEnd), brushSize);
        }
        
        /*// 마우스 좌클릭으로 선택 시작
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                selectionStart = new Vector2(x, y);
                isSelecting = true;
            }
        }

        // 마우스 드래그로 선택 확장
        if (Input.GetMouseButton(0) && isSelecting)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint))
            {
                float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
                float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
                selectionEnd = new Vector2(x, y);
                UpdateSelectionRect();
            }
        }

        // 마우스 버튼을 떼면 선택 완료
        if (Input.GetMouseButtonUp(0) && isSelecting)
        {
            isSelecting = false;
            Debug.Log($"선택된 영역: {selectionRect}");
        }*/
    }
    
    void DrawSelectionBorder(Vector2 start, Vector2 end, Color borderColor, float thickness)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 상단 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMax, yMin), borderColor, thickness);
        // 하단 변
        DrawThickLineForSelect(new Vector2(xMin, yMax), new Vector2(xMax, yMax), borderColor, thickness);
        // 왼쪽 변
        DrawThickLineForSelect(new Vector2(xMin, yMin), new Vector2(xMin, yMax), borderColor, thickness);
        // 오른쪽 변
        DrawThickLineForSelect(new Vector2(xMax, yMin), new Vector2(xMax, yMax), borderColor, thickness);

        drawTexture.Apply(); // 변경 사항 적용
    }

    void ClearSelectionArea(Vector2 start, Vector2 end, float thickness)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 내부 지우기
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
                {
                    drawTexture.SetPixel(x, y, Color.white);
                }
            }
        }

        // 테두리만 다시 덮어씌움
        // 상단 변
        DrawThickLine(new Vector2(xMin, yMin), new Vector2(xMax, yMin), Color.white, thickness);
        // 하단 변
        DrawThickLine(new Vector2(xMin, yMax), new Vector2(xMax, yMax), Color.white, thickness);
        // 왼쪽 변
        DrawThickLine(new Vector2(xMin, yMin), new Vector2(xMin, yMax), Color.white, thickness);
        // 오른쪽 변
        DrawThickLine(new Vector2(xMax, yMin), new Vector2(xMax, yMax), Color.white, thickness);
        
        // 꼭짓점 부분 정확히 지우기
        ClearCorner(new Vector2(xMin, yMin), thickness);
        ClearCorner(new Vector2(xMax, yMin), thickness);
        ClearCorner(new Vector2(xMin, yMax), thickness);
        ClearCorner(new Vector2(xMax, yMax), thickness);
        
        drawTexture.Apply();
    }
    
    void ClearCorner(Vector2 corner, float thickness)
    {
        int cx = Mathf.RoundToInt(corner.x);
        int cy = Mathf.RoundToInt(corner.y);

        for (int dx = -Mathf.CeilToInt(thickness / 2); dx <= Mathf.FloorToInt(thickness / 2); dx++)
        {
            for (int dy = -Mathf.CeilToInt(thickness / 2); dy <= Mathf.FloorToInt(thickness / 2); dy++)
            {
                int px = cx + dx;
                int py = cy + dy;

                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    drawTexture.SetPixel(px, py, Color.white);
                }
            }
        }
    }
    
    void DrawThickLineForSelect(Vector2 start, Vector2 end, Color color, float thickness)
    {
        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            for (int tx = -Mathf.CeilToInt(thickness / 2); tx <= Mathf.FloorToInt(thickness / 2); tx++)
            {
                for (int ty = -Mathf.CeilToInt(thickness / 2); ty <= Mathf.FloorToInt(thickness / 2); ty++)
                {
                    int px = x0 + tx;
                    int py = y0 + ty;

                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        drawTexture.SetPixel(px, py, color);
                    }
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
    
    void UpdateSelectionRect()
    {
        // 시작점과 끝점을 기반으로 영역 정의
        float xMin = Mathf.Min(selectionStart.x, selectionEnd.x);
        float xMax = Mathf.Max(selectionStart.x, selectionEnd.x);
        float yMin = Mathf.Min(selectionStart.y, selectionEnd.y);
        float yMax = Mathf.Max(selectionStart.y, selectionEnd.y);

        selectionRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

        // 선택 영역 시각화
        VisualizeSelection();
    }

    void VisualizeSelection()
    {
        // 텍스처에 선택 영역을 반투명하게 표시
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                if (x >= selectionRect.xMin && x <= selectionRect.xMax &&
                    y >= selectionRect.yMin && y <= selectionRect.yMax)
                {
                    Color originalColor = drawTexture.GetPixel(x, y);
                    drawTexture.SetPixel(x, y, Color.Lerp(originalColor, selectionColor, 0.5f));
                }
            }
        }
        drawTexture.Apply();
    }

    public void ClearSelection()
    {
        // 선택 영역 제거
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                if (x >= selectionRect.xMin && x <= selectionRect.xMax &&
                    y >= selectionRect.yMin && y <= selectionRect.yMax)
                {
                    drawTexture.SetPixel(x, y, Color.white);
                }
            }
        }
        drawTexture.Apply();

        selectionRect = Rect.zero; // 영역 초기화
    }
    
    void DrawAt(int x, int y)
    {
        // 브러시 영역에 색상 적용
        for (int i = -Mathf.FloorToInt(brushSize); i < Mathf.CeilToInt(brushSize); i++)
        {
            for (int j = -Mathf.FloorToInt(brushSize); j < Mathf.CeilToInt(brushSize); j++)
            {
                int px = x + i;
                int py = y + j;

                // 텍스처 범위를 벗어나지 않도록 제한
                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(px, py));
                    if (distance <= brushSize)
                    {
                        drawTexture.SetPixel(px, py, drawColor);
                    }
                }
            }
        }
        drawTexture.Apply(); // 변경 사항 적용
    }
    
    void DrawBrush(int x, int y)
    {
        Texture2D currentTexture = GetCurrentBrushTexture();

        for (int i = -Mathf.FloorToInt(brushSize); i < Mathf.CeilToInt(brushSize); i++)
        {
            for (int j = -Mathf.FloorToInt(brushSize); j < Mathf.CeilToInt(brushSize); j++)
            {
                int px = x + i;
                int py = y + j;

                if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(px, py));
                    if (distance <= brushSize)
                    {
                        // 브러쉬 질감 적용
                        float alpha = currentTexture.GetPixelBilinear((float)(i + brushSize) / (brushSize * 2),
                                                                      (float)(j + brushSize) / (brushSize * 2)).a;
                        Color currentColor = drawTexture.GetPixel(px, py);
                        Color blendedColor = Color.Lerp(currentColor, drawColor, alpha);
                        drawTexture.SetPixel(px, py, blendedColor);
                    }
                }
            }
        }
        drawTexture.Apply();
    }

    Texture2D GetCurrentBrushTexture()
    {
        switch (currentBrush)
        {
            case BrushType.Pencil: return pencilTexture;
            case BrushType.Pen: return penTexture;
            case BrushType.Crayon: return crayonTexture;
            default: return drawTexture;
        }
    }

    void SetBrushType(BrushType brushType)
    {
        currentBrush = brushType;
        Debug.Log($"브러쉬 변경: {brushType}");
    }

    void LoadBrushTextures()
    {
        // 각 브러쉬 질감 로드 (Unity Editor의 Resources 폴더에 미리 저장된 텍스처)
        pencilTexture = Resources.Load<Texture2D>("BrushTextures/Pencil");
        penTexture = Resources.Load<Texture2D>("BrushTextures/Pen");
        crayonTexture = Resources.Load<Texture2D>("BrushTextures/Crayon");

        if (!pencilTexture || !penTexture || !crayonTexture)
        {
            Debug.LogError("브러쉬 질감 로드 실패: Resources/BrushTextures 폴더에 텍스처를 추가하세요.");
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        // 선을 따라가며 픽셀 그리기
        while (true)
        {
            DrawThickPixel(x0, y0, drawColor, brushSize); // 두께 적용

            if (x0 == x1 && y0 == y1) break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        drawTexture.Apply();
    }

    void DrawRectangle(Vector2 start, Vector2 end)
    {
        int xMin = Mathf.RoundToInt(Mathf.Min(start.x, end.x));
        int xMax = Mathf.RoundToInt(Mathf.Max(start.x, end.x));
        int yMin = Mathf.RoundToInt(Mathf.Min(start.y, end.y));
        int yMax = Mathf.RoundToInt(Mathf.Max(start.y, end.y));

        // 두께를 적용한 선 그리기
        DrawThickLine(new Vector2(xMin, yMin), new Vector2(xMax, yMin), drawColor, brushSize); // 상단 변
        DrawThickLine(new Vector2(xMax, yMin), new Vector2(xMax, yMax), drawColor, brushSize); // 오른쪽 변
        DrawThickLine(new Vector2(xMax, yMax), new Vector2(xMin, yMax), drawColor, brushSize); // 하단 변
        DrawThickLine(new Vector2(xMin, yMax), new Vector2(xMin, yMin), drawColor, brushSize); // 왼쪽 변
    }

    // 두께를 적용한 선 그리기
    void DrawThickLine(Vector2 start, Vector2 end, Color color, float thickness)
    {
        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawThickPixel(x0, y0, color, thickness); // 두께를 적용한 픽셀 그리기

            if (x0 == x1 && y0 == y1) break;

            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
        drawTexture.Apply();
    }

    // 두께 적용된 픽셀 그리기
    void DrawThickPixel(int x, int y, Color color, float thickness)
    {
        int radius = Mathf.CeilToInt(thickness / 2);
        for (int offsetX = -radius; offsetX <= radius; offsetX++)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                // 두께를 원형으로 처리
                if (Vector2.Distance(Vector2.zero, new Vector2(offsetX, offsetY)) <= radius)
                {
                    int px = x + offsetX;
                    int py = y + offsetY;

                    // 텍스처 범위 확인
                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        drawTexture.SetPixel(px, py, color);
                    }
                }
            }
        }
    }
    
    void DrawCircle(Vector2 start, Vector2 end)
    {
        float radius = Vector2.Distance(start, end) / 2;
        Vector2 center = (start + end) / 2;

        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
            
                // 반지름에 가까운 거리만 픽셀 설정
                if (Mathf.Abs(distance - radius) <= brushSize) // 원 둘레 두께
                {
                    drawTexture.SetPixel(x, y, drawColor);
                }
            }
        }
        drawTexture.Apply();
    }
    
    Vector2 ScreenToTextureCoord(Vector2 screenPosition)
    {
        RectTransform rectTransform = targetImage.GetComponent<RectTransform>();
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPoint))
        {
            float x = (localPoint.x + rectTransform.rect.width / 2) / rectTransform.rect.width * textureWidth;
            float y = (localPoint.y + rectTransform.rect.height / 2) / rectTransform.rect.height * textureHeight;
            return new Vector2(Mathf.Clamp(x, 0, textureWidth - 1), Mathf.Clamp(y, 0, textureHeight - 1));
        }

        return Vector2.zero;
    }

    void SetDrawMode(DrawMode mode)
    {
        currentMode = mode;
        Debug.Log($"모드 변경: {mode}");
    }

    void SaveTexture()
    {
        // 텍스처를 PNG 데이터로 변환
        byte[] pngData = drawTexture.EncodeToPNG();
        if (pngData != null)
        {
            // 파일 저장 경로 설정
            string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

            // PNG 데이터를 파일로 저장
            File.WriteAllBytes(filePath, pngData);
            
            // 저장 알림 메시지 띄우기
            UIManager.Instance.RunPopupCoroutine("그림이 저장되었습니다!");
            
            // 저장 완료 로그
            Debug.Log($"그림이 저장되었습니다: {filePath}");
        }
        else
        {
            Debug.LogError("PNG 데이터를 생성하는 데 실패했습니다.");
        }
    }
    
    void LoadTexture()
    {
        // 파일 경로 설정
        string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

        if (File.Exists(filePath))
        {
            // 저장된 파일 읽기
            byte[] pngData = File.ReadAllBytes(filePath);

            // 텍스처에 로드
            if (drawTexture != null)
            {
                drawTexture.LoadImage(pngData);
                drawTexture.Apply();
                UIManager.Instance.RunPopupCoroutine("그림이 성공적으로 불러와졌습니다.");
                Debug.Log("그림이 성공적으로 불러와졌습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"저장된 그림이 없습니다: {filePath}");
            UIManager.Instance.RunPopupCoroutine("저장된 그림이 없습니다!");
        }
    }

    public void OnClickSendButton()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SavedDrawing.png");

        if (File.Exists(filePath))
        {
            APIManager.Instance.SendRequest();
            UIManager.Instance.RunPopupCoroutine("그림이 전송됐습니다!");
        }
        else
        {
            Debug.LogWarning("저장된 그림이 없습니다!");
            UIManager.Instance.RunPopupCoroutine("저장된 그림이 없습니다!");
        }
    }
    
    void ChangeColor(Color newColor)
    {
        drawColor = newColor;
        Debug.Log($"브러쉬 색상이 변경되었습니다: {newColor}");
    }

    void ChangeBrushSize(float newSize)
    {
        brushSize = newSize;
        Debug.Log($"브러쉬 크기가 변경되었습니다: {newSize}");
    }
    
    // 현재 상태 저장 (Undo)
    void SaveUndoState()
    {
        Color[] currentPixels = drawTexture.GetPixels();
        undoStack.Push(currentPixels);
        redoStack.Clear(); // 새 작업이 시작되면 Redo 스택 초기화
    }

    // Undo 기능
    void Undo()
    {
        if (undoStack.Count > 0)
        {
            Color[] lastState = undoStack.Pop();
            redoStack.Push(drawTexture.GetPixels()); // 현재 상태를 Redo 스택으로 이동
            ApplyState(lastState);
        }
    }

    // Redo 기능
    void Redo()
    {
        if (redoStack.Count > 0)
        {
            Color[] nextState = redoStack.Pop();
            undoStack.Push(drawTexture.GetPixels()); // 현재 상태를 Undo 스택으로 이동
            ApplyState(nextState);
        }
    }

    // 텍스처 상태 적용
    void ApplyState(Color[] pixelColors)
    {
        drawTexture.SetPixels(pixelColors);
        drawTexture.Apply();
    }
    
    public void OnClickLoadButton()
    {
        LoadTexture();
    }
    
    public void OnClickSaveButton()
    {
        SaveTexture();
    }

    public void OnClickReturnButton()
    {
        gameObject.SetActive(false);
    }
}
