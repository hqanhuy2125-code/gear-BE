import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.font_manager import FontProperties
import os

# Set font for Vietnamese support
font_path = r'C:\Windows\Fonts\arial.ttf'
if not os.path.exists(font_path):
    font_path = 'DejaVu Sans'

prop = FontProperties(fname=font_path)
bold_prop = FontProperties(fname=font_path, weight='bold')

def draw_actor(ax, x, y, name):
    # Stick figure
    ax.add_patch(patches.Circle((x, y + 0.5), 0.2, fill=False, ec='black', lw=1.5)) # Head
    ax.plot([x, x], [y + 0.3, y - 0.3], color='black', lw=1.5) # Body
    ax.plot([x - 0.3, x + 0.3], [y + 0.1, y + 0.1], color='black', lw=1.5) # Arms
    ax.plot([x, x - 0.2], [y - 0.3, y - 0.7], color='black', lw=1.5) # Left leg
    ax.plot([x, x + 0.2], [y - 0.3, y - 0.7], color='black', lw=1.5) # Right leg
    ax.text(x, y - 1.0, name, ha='center', va='top', fontproperties=bold_prop, fontsize=12)

def draw_usecase(ax, x, y, name):
    ellipse = patches.Ellipse((x, y), 3.0, 0.8, fill=False, ec='black', lw=1.2)
    ax.add_patch(ellipse)
    ax.text(x, y, name, ha='center', va='center', fontproperties=prop, fontsize=10, wrap=True)

def main():
    fig, ax = plt.subplots(figsize=(24, 18))
    ax.set_xlim(0, 30)
    ax.set_ylim(0, 20)
    ax.set_aspect('equal')
    ax.axis('off')
    
    # System boundary
    rect = patches.Rectangle((5, 1), 20, 18, fill=False, ec='black', lw=2, linestyle='--')
    ax.add_patch(rect)
    ax.text(15, 19.5, "Hệ thống GamingGear", ha='center', va='top', fontproperties=bold_prop, fontsize=18)
    
    # Actors
    actors = {
        'Guest': (2, 16, 'Khách vãng lai'),
        'Customer': (2, 9, 'Khách hàng'),
        'Admin': (27, 10, 'Quản trị viên'),
        'Shipper': (27, 3, 'Shipper / Driver')
    }
    
    for a, data in actors.items():
        draw_actor(ax, data[0], data[1], data[2])
        
    # Use cases (pos_y starts from top)
    ucs = [
        (10, 17.5, 'Xem/Tìm kiếm/Lọc sản phẩm'),
        (10, 16.5, 'Xem bài viết Blog'),
        (10, 15.0, 'Đăng nhập / Đăng ký'),
        (10, 13.5, 'Quản lý giỏ hàng'),
        (10, 12.0, 'Đặt hàng & Thanh toán'),
        (10, 10.5, 'Xem lịch sử đơn hàng'),
        (10, 9.0, 'Quản lý Wishlist'),
        (10, 7.5, 'Đánh giá sản phẩm'),
        (10, 6.0, 'Chat hỗ trợ / Nhận thông báo'),
        
        (20, 17.5, 'Quản lý Sản phẩm / Blog'),
        (20, 16.0, 'Quản lý Đơn hàng / Driver'),
        (20, 14.5, 'Quản lý Voucher / Promo'),
        (20, 13.0, 'Quản lý Người dùng'),
        (20, 11.5, 'Báo cáo thống kê'),
        
        (20, 4.0, 'Nhận đơn / Cập nhật giao hàng')
    ]
    
    for x, y, name in ucs:
        draw_usecase(ax, x, y, name)
        
    # Connections (Actors to Use Cases)
    def link(actor_pos, uc_pos):
        ax.plot([actor_pos[0], uc_pos[0]], [actor_pos[1], uc_pos[1]], color='gray', lw=1, alpha=0.6)

    # Guest
    link((2.5, 16.5), (8.5, 17.5))
    link((2.5, 16.5), (8.5, 16.5))
    
    # Customer (inherits Guest potentially, but explicitly linked here)
    link((2.5, 9.5), (8.5, 17.5))
    link((2.5, 9.5), (8.5, 15.0))
    link((2.5, 9.5), (8.5, 13.5))
    link((2.5, 9.5), (8.5, 12.0))
    link((2.5, 9.5), (8.5, 10.5))
    link((2.5, 9.5), (8.5, 9.0))
    link((2.5, 9.5), (8.5, 7.5))
    link((2.5, 9.5), (8.5, 6.0))
    
    # Admin
    link((26.5, 10.5), (21.5, 17.5))
    link((26.5, 10.5), (21.5, 16.0))
    link((26.5, 10.5), (21.5, 14.5))
    link((26.5, 10.5), (21.5, 13.0))
    link((26.5, 10.5), (21.5, 11.5))
    
    # Shipper
    link((26.5, 3.5), (21.5, 4.0))

    plt.title("GamingGear - Sơ đồ Use Case", fontproperties=bold_prop, fontsize=22, pad=30)
    plt.tight_layout()
    output_path = r'd:\Hà Quang Huy_231A290063_WebNC\UseCase_Diagram.png'
    plt.savefig(output_path, dpi=150, bbox_inches='tight')
    print(f"Success: {output_path}")

if __name__ == "__main__":
    main()
