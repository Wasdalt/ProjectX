import argparse
import pyscreenshot as ImageGrab

def capture_screenshot(x1, y1, x2, y2, output_path):
    try:
        im = ImageGrab.grab(bbox=(x1, y1, x2, y2), backend="pil", childprocess=False)
        im.save(output_path)
        return True
    except Exception:
        return False

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Capture a screenshot of a specified region.')
    parser.add_argument('x1', type=int, help='X coordinate of the top-left corner')
    parser.add_argument('y1', type=int, help='Y coordinate of the top-left corner')
    parser.add_argument('x2', type=int, help='X coordinate of the bottom-right corner')
    parser.add_argument('y2', type=int, help='Y coordinate of the bottom-right corner')
    parser.add_argument('output_path', help='Path to save the screenshot')

    args = parser.parse_args()

    result = capture_screenshot(args.x1, args.y1, args.x2, args.y2, args.output_path)
    exit(0 if result else 1)
