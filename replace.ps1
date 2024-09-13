# 获取用户输入的替换字符串
$newString = Read-Host "请输入要替换 'FlashDotNet' 的新字符串"

# 获取当前目录下所有目录，按深度排序（最深的先处理）
$directories = Get-ChildItem -Path . -Directory -Recurse | 
    Sort-Object -Property @{Expression={$_.FullName.Split([IO.Path]::DirectorySeparatorChar).Count}; Descending=$true}

# 处理目录名
foreach ($dir in $directories) {
    if ($dir.Name -like "*FlashDotNet*") {
        $newName = $dir.Name -replace "FlashDotNet", $newString
        $newPath = Join-Path -Path $dir.Parent.FullName -ChildPath $newName
        Rename-Item -Path $dir.FullName -NewName $newName
        Write-Host "目录已重命名: $($dir.FullName) -> $newPath"
    }
}

# 处理文件名
Get-ChildItem -Path . -File -Recurse | ForEach-Object {
    if ($_.Name -like "*FlashDotNet*") {
        $newName = $_.Name -replace "FlashDotNet", $newString
        $newPath = Join-Path -Path $_.DirectoryName -ChildPath $newName
        Rename-Item -Path $_.FullName -NewName $newName
        Write-Host "文件已重命名: $($_.FullName) -> $newPath"
    }
}

# 处理文件内容
Get-ChildItem -Path . -File -Recurse | ForEach-Object {
    $content = Get-Content -Path $_.FullName -Raw -Encoding UTF8
    if ($content -like "*FlashDotNet*") {
        $newContent = $content -replace "FlashDotNet", $newString
        [System.IO.File]::WriteAllText($_.FullName, $newContent, [System.Text.Encoding]::UTF8)
        Write-Host "文件内容已更新: $($_.FullName)"
    }
}

Write-Host "处理完成。"
