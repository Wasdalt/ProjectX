import pytesseract
from PIL import Image

def ocr_image(image_path):
    # Открываем изображение с помощью PIL
    img = Image.open(image_path)

    # Используем pytesseract для распознавания текста
    text = pytesseract.image_to_string(img, lang='eng')

    # Возвращаем распознанный текст
    return text

# Пример использования
image_path = '/home/nubuck/Документы/123.png'
text = ocr_image(image_path)
print(text)