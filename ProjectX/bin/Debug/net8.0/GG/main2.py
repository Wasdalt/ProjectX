import os
import sys
import argparse
import pyscreenshot as ImageGrab

def capture_screenshot(output_path):
    try:
        # Создаем директорию, если она не существует
        os.makedirs(os.path.dirname(output_path), exist_ok=True)

        # Используем ImageGrab.grab для захвата скриншота
        im = ImageGrab.grab(backend="pil")
        im.save(output_path)

        return True
    except Exception as e:
        print(f"Ошибка при создании скриншота: {e}", file=sys.stderr)
        return False

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Capture a screenshot of all screens.')
    parser.add_argument('output_path', help='Path to save the screenshot')

    args = parser.parse_args()

    result = capture_screenshot(args.output_path)
    sys.exit(0 if result else 1)

