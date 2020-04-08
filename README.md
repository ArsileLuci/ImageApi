# ImageApi

Установить `.NET Core версии 3.1` 
https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-package-manager-ubuntu-1804

Для сборки:
перейти в папку с проектом ./ImageApi/ImageApi и вызвать `dotnet build`

Для запуска тестов:
Перейтив папку к *.sln файлу и вызвать `dotnet test`

Для запуска сервера:
перейти в папку с проектом ./ImageApi/ImageApi и вызвать `dotnet run`

Сервер работает на адресах localhost:5000 и localhost:5001

# Доступные методы:

## /Image/{id} GET
возвращает объект с массивом массив байтов соответствующих изображению
при отсутствии изображения в базе 

## /Preview/{id} GET
возвращает объект с массивом массив байтов соответствующих превью изображения, размером 100 на 100 пикселей
изображение сжимается(растягивается) до необходимого размера с сохранением пропорций после чего обрезается до необходимого размера

## /AddBase64 POST

### Body Example: 
`{"Base64":"<base64 image representation>"}`
заголовок base64 не имеет значения, проверка на изображение проверяется по метадате файла
### Response Example:
`{"Id": 1}`
где `Id` это идентификатор изображения в базе
### Erors:
BadRequest: переданная строка была пустая
            переданная строка не в формате base64,
            формат изображения не поддерживается

## /AddFromUrl POST

### Body Example: 
`{"Url":"<url адрес ресурса>"}`
### Response Example:
`{"Id": 1}`
где `Id` это идентификатор изображения в базе
### Erors:
BadRequest: не возможно получить данные по заданному Url,
            формат изображения не поддерживается

## /AddMultipart POST

### Body Example: 
<Multipart Body with one file>

### Response Example:
`{"Id": 1}`
BadRequest: формат изображения не поддерживается


## /LoadImages POST

### Body Example: 
<Multipart Body with one or multiple files>

### Response Example:
`[{"Id": 1}, null, {"Id":2}]`
