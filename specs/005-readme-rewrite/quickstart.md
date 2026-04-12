# Quickstart: README.md 重寫

**Feature**: 005-readme-rewrite  
**Date**: 2026-04-12

## 實作步驟

### Step 1: 備份現有 README.md
```powershell
Copy-Item docs/README.md docs/README.md.bak
```

### Step 2: 重寫 docs/README.md
以 `data-model.md` 定義的結構重寫，包含：
1. 標題與簡介
2. 技術棧（前端 Vue 3 / Element Plus / Pinia / Tailwind；後端 .NET 8 / EF Core / JWT）
3. 快速啟動（前後端指令）
4. 功能模組與 API 控制器對照（7 個 Controller）
5. 文件索引（11 大分類，65 個檔案全覆蓋）
6. 專案目錄結構
7. 推薦閱讀順序
8. 最後更新日期

### Step 3: 驗證連結
```powershell
# 驗證所有連結指向存在的檔案
$readme = Get-Content docs/README.md -Raw
$links = [regex]::Matches($readme, '\]\(([^)]+\.md[^)]*)\)') | ForEach-Object { $_.Groups[1].Value }
$broken = $links | Where-Object { -not (Test-Path "docs/$_") }
if ($broken) { Write-Error "Broken links: $($broken -join ', ')" }
```

### Step 4: 驗證覆蓋率
```powershell
# 確認 docs/ 下每個 .md 都被索引
$allDocs = Get-ChildItem docs -Recurse -Filter *.md | Where-Object { $_.Name -ne 'README.md' }
$missing = $allDocs | Where-Object { $readme -notmatch [regex]::Escape($_.Name) }
if ($missing) { Write-Error "Missing from index: $($missing.Name -join ', ')" }
```

## 驗收標準

| 標準 | 驗證方式 |
|------|----------|
| 100% 連結正確 | Step 3 PowerShell 腳本零錯誤 |
| 100% 檔案覆蓋 | Step 4 PowerShell 腳本零遺漏 |
| 系統簡介完整 | 包含書籍、使用者、角色、選單、認證五大功能 |
| 技術棧準確 | 版本號與 package.json / .csproj 一致 |
| 推薦標示 | LOGIN、SYSMENU、DYNAMIC_ROUTING 等系列有 ⭐ |

## 預估範圍

- 修改檔案：1 個（`docs/README.md`）
- 新增檔案：0 個
- 刪除檔案：0 個
