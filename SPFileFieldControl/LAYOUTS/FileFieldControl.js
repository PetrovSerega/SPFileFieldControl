// Отображение/скрытие элемента по ИД
function changeDisplay(id) {
    var v = document.getElementById(id);
    if (v.style.display == 'block') //|| v.style.display == '') 
    {
        v.style.display = 'none';
    }
    else {
        v.style.display = 'block';
    }
}

// Очистка ссылки на файл, скрытого поля и поля загрузки файла
function clearFileValue(aID, fID, hfId, defText) {
    var a = document.getElementById(aID);
    a.innerText = defText;
    a.removeAttribute("href");

    var hf = document.getElementById(hfId);
    hf.value = '';

    var f = document.getElementById(fID);
    f.outerHTML = f.outerHTML;
}

// Обработка изменения файла
function changeFileName(oFile, aID) {
    var a = document.getElementById(aID);

    var fullPath = oFile.value;
    if (fullPath)
    {
        // В гиперсылке покажем имя файла без полного пути к нему
        var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
        var filename = fullPath.substring(startIndex);
        if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
            filename = filename.substring(1);
        }
        
        a.innerText = filename;
    }
}