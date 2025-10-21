# 工廠管理系統 - 在線生產看板

本專案提供一個以 Vue 3 為前端、.NET 6 Web API 為後端的在線生產看板。使用者可以依廠區、部、課、班等條件查詢來自 MSSQL 2008 的即時人員數據。

## 專案結構

```
backend/ProductionBoardApi   # .NET 6 Web API 專案
frontend/                    # Vue 3 (Vite) 單頁應用
```

## 主要功能

- 於前端介面選擇廠區（珠海、越南、太倉）、部別、課別與班別。
- 呼叫後端 API 以參數化查詢資料庫，回傳指定部門的在線人數、應出席、實際出席、請假、借出、借入等資訊。
- 前端即時呈現查詢結果，並提供錯誤處理與載入狀態顯示。

## 後端服務啟動

1. 安裝 .NET 6 SDK。
2. 於 `backend/ProductionBoardApi` 目錄執行：
   ```bash
   dotnet restore
   dotnet run
   ```

   預設服務在 `https://localhost:5001`（或 `http://localhost:5000`）啟動。

> ⚠️ 需確認 `appsettings.json` 中的資料庫連線字串是否符合實際環境，並確保 API 主機能連線至 MSSQL 伺服器。

## 前端專案啟動

1. 安裝 Node.js 18+。
2. 於 `frontend` 目錄執行：
   ```bash
   npm install
   npm run dev
   ```

   Vite 開發伺服器預設在 `http://localhost:5173` 啟動，並會透過開發代理轉發 `/api` 請求至 `http://localhost:5000`。

3. 建置正式檔案：
   ```bash
   npm run build
   ```

## API 範例

```http
GET /api/ProductionBoard?plant=ZH&division=1&section=1&classNumber=1
```

回應內容：

```json
[
  {
    "departmentCode": "ZH-1-1-1",
    "departmentName": "1課1班",
    "onlineCount": 30,
    "expectedAttendance": 35,
    "actualAttendance": 32,
    "borrowedCount": 1,
    "lentCount": 2,
    "leaveCount": 3
  }
]
```

> 實際資料依資料庫查詢結果而定。

## 授權

本專案僅供內部系統整合與示範之用。
