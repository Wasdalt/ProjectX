import argparse
import os
import sys
import subprocess

def capture_screenshot(output_path):
    try:
        # Создаем директорию, если она не существует
        os.makedirs(os.path.dirname(output_path), exist_ok=True)

        # Используем gnome-screenshot для захвата скриншота
        result = subprocess.run(['gnome-screenshot', '-f', output_path], capture_output=True, text=True)

        if result.returncode != 0:
            print(f"Ошибка при создании скриншота: {result.stderr}", file=sys.stderr)
            return False

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

