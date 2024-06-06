from PIL import Image, ImageOps, ImageEnhance
import pytesseract
import cv2
import numpy as np
import sys

def recognize_text(image_path):
    # Открываем изображение
    image = Image.open(image_path)

    # Преобразуем изображение в оттенки серого
    gray_image = ImageOps.grayscale(image)

    # Вычисляем среднюю яркость изображения
    histogram = gray_image.histogram()
    pixels = sum(histogram)
    brightness = scale = len(histogram)
    avg_brightness = sum(i * histogram[i] for i in range(scale)) / pixels

    # Инвертируем изображение, если оно слишком темное
    if avg_brightness < 128:
        processed_image = ImageOps.invert(gray_image)
    else:
        processed_image = gray_image

    # Увеличиваем контраст изображения
    enhancer = ImageEnhance.Contrast(processed_image)
    high_contrast_image = enhancer.enhance(2)

    # Увеличиваем размер изображения
    larger_image = high_contrast_image.resize((high_contrast_image.width * 2, high_contrast_image.height * 2))

    # Бинаризуем изображение
    _, binary_image = cv2.threshold(np.array(larger_image), 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)

    # Конвертируем обратно в изображение PIL
    binary_pil_image = Image.fromarray(binary_image)

    # Конфигурация для распознавания специальных символов
    custom_config = r'--oem 1 --psm 6'

    # Распознаем текст на русском и английском языках
    text = pytesseract.image_to_string(binary_pil_image, lang='eng+rus', config=custom_config)

    return text

# Основной блок для выполнения скрипта
if __name__ == "__main__":
    # Проверка наличия аргументов командной строки
    if len(sys.argv) < 2:
        print("Использование: python script.py <путь_к_изображению>")
        sys.exit(1)

    image_path = sys.argv[1]

    # Установка пути к Tesseract для Windows
    if sys.platform == "win32":
        pytesseract.pytesseract.tesseract_cmd = 'Tesseract-OCR/tesseract.exe'

    recognized_text = recognize_text(image_path)
    print(recognized_text)

