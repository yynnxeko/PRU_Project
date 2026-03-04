from PIL import Image, ImageDraw
import os

output_dir = "Assets/Sprites"

def create_door_sprite():
    """Door - 16×32 px (2 tiles high)"""
    img = Image.new('RGBA', (16, 32), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Door frame
    draw.rectangle([2, 0, 13, 31], fill=(100, 70, 40, 255))
    draw.rectangle([2, 0, 13, 31], outline=(60, 40, 20, 255), width=1)
    # Door handle
    draw.rectangle([11, 16, 12, 18], fill=(200, 200, 0, 255))
    img.save(f"{output_dir}/door_placeholder.png")
    print("✓ door_placeholder.png (16×32)")

def create_key_sprite():
    """Key - 16×16 px"""
    img = Image.new('RGBA', (16, 16), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Key head
    draw.ellipse([4, 4, 9, 9], fill=(200, 200, 0, 255))
    draw.ellipse([5, 5, 8, 8], fill=(0, 0, 0, 0))  # hole
    # Key shaft
    draw.rectangle([9, 6, 12, 7], fill=(200, 200, 0, 255))
    # Key teeth
    draw.rectangle([11, 7, 12, 9], fill=(200, 200, 0, 255))
    img.save(f"{output_dir}/key_placeholder.png")
    print("✓ key_placeholder.png (16×16)")

def create_vent_sprite():
    """Air vent - 32×16 px"""
    img = Image.new('RGBA', (32, 16), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Vent frame
    draw.rectangle([0, 2, 31, 13], fill=(60, 60, 60, 255))
    # Vent slats
    for y in range(4, 12, 2):
        draw.line([2, y, 29, y], fill=(40, 40, 40, 255))
    img.save(f"{output_dir}/vent_placeholder.png")
    print("✓ vent_placeholder.png (32×16)")

def create_light_sprite():
    """Light/Lamp - 16×16 px"""
    img = Image.new('RGBA', (16, 16), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Bulb
    draw.ellipse([5, 6, 10, 12], fill=(255, 255, 200, 255))
    # Socket
    draw.rectangle([6, 12, 9, 14], fill=(80, 80, 80, 255))
    # Light rays
    draw.line([8, 3, 8, 5], fill=(255, 255, 150, 180))
    draw.line([3, 8, 5, 8], fill=(255, 255, 150, 180))
    draw.line([10, 8, 12, 8], fill=(255, 255, 150, 180))
    img.save(f"{output_dir}/light_placeholder.png")
    print("✓ light_placeholder.png (16×16)")

def create_crate_sprite():
    """Crate/Box - 16×16 px"""
    img = Image.new('RGBA', (16, 16), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # Box
    draw.rectangle([2, 2, 13, 13], fill=(150, 100, 50, 255))
    # Wood lines
    draw.line([2, 7, 13, 7], fill=(120, 80, 40, 255))
    draw.line([7, 2, 7, 13], fill=(120, 80, 40, 255))
    # Border
    draw.rectangle([2, 2, 13, 13], outline=(100, 60, 30, 255), width=1)
    img.save(f"{output_dir}/crate_placeholder.png")
    print("✓ crate_placeholder.png (16×16)")

if __name__ == "__main__":
    print("\n🎨 Tạo thêm placeholder sprites...\n")
    create_door_sprite()
    create_key_sprite()
    create_vent_sprite()
    create_light_sprite()
    create_crate_sprite()
    print("\n✅ Xong! Chạy lại Unity để import.\n")
