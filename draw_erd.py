import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.font_manager import FontProperties
import os

# Set font for Vietnamese support (Arial is common on Windows)
font_path = r'C:\Windows\Fonts\arial.ttf'
if not os.path.exists(font_path):
    font_path = 'DejaVu Sans' # fallback

prop = FontProperties(fname=font_path)
bold_prop = FontProperties(fname=font_path, weight='bold')

def draw_table(ax, x, y, name, columns, pks, fks):
    width = 3.5
    row_height = 0.35
    header_height = 0.5
    table_height = header_height + len(columns) * row_height
    
    # Draw header
    ax.add_patch(patches.Rectangle((x, y - header_height), width, header_height, color='#e0e0e0', ec='black', lw=1.5))
    ax.text(x + width/2, y - header_height/2, name, ha='center', va='center', fontproperties=bold_prop, fontsize=12)
    
    # Draw body
    ax.add_patch(patches.Rectangle((x, y - table_height), width, len(columns) * row_height, color='white', ec='black', lw=1.5))
    
    for i, col in enumerate(columns):
        col_y = y - header_height - (i + 0.5) * row_height
        
        # Determine color for PK
        color = 'black'
        text_prop = prop
        if col in pks:
            color = 'red'
            text_prop = bold_prop
        
        # Label FK
        label = col
        if col in fks:
            label += " (FK)"
            
        ax.text(x + 0.2, col_y, label, ha='left', va='center', color=color, fontproperties=text_prop, fontsize=10)
        
    return x + width/2, y - table_height/2, width, table_height

def main():
    fig, ax = plt.subplots(figsize=(24, 18))
    ax.set_aspect('equal')
    ax.axis('off')
    
    # Define table positions (manually tuned for 15 tables)
    tables = {
        'Users': {'pos': (10, 15), 'cols': ['Id', 'Name', 'Email', 'Password', 'Role', 'CreatedAt'], 'pks': ['Id'], 'fks': []},
        'Products': {'pos': (2, 10), 'cols': ['Id', 'Name', 'Description', 'Price', 'ImageUrl', 'Category', 'Stock', 'IsPreOrder', 'PreOrderDate', 'IsOrderOnly', 'CreatedAt'], 'pks': ['Id'], 'fks': []},
        'Orders': {'pos': (18, 10), 'cols': ['Id', 'UserId', 'TotalPrice', 'Status', 'FullName', 'PhoneNumber', 'ShippingAddress', 'PaymentMethod', 'CreatedAt'], 'pks': ['Id'], 'fks': ['UserId']},
        'OrderItems': {'pos': (10, 8), 'cols': ['Id', 'OrderId', 'ProductId', 'Quantity', 'Price'], 'pks': ['Id'], 'fks': ['OrderId', 'ProductId']},
        'CartItems': {'pos': (2, 15), 'cols': ['Id', 'UserId', 'ProductId', 'Quantity'], 'pks': ['Id'], 'fks': ['UserId', 'ProductId']},
        'WishlistItems': {'pos': (2, 4), 'cols': ['Id', 'UserId', 'ProductId', 'CreatedAt'], 'pks': ['Id'], 'fks': ['UserId', 'ProductId']},
        'Reviews': {'pos': (10, 2), 'cols': ['Id', 'ProductId', 'UserId', 'Rating', 'Comment', 'NickName', 'CreatedAt'], 'pks': ['Id'], 'fks': ['ProductId', 'UserId']},
        'Blogs': {'pos': (18, 15), 'cols': ['Id', 'Title', 'Content', 'ImageUrl', 'Author', 'CreatedAt'], 'pks': ['Id'], 'fks': []},
        'Promotions': {'pos': (18, 4), 'cols': ['Id', 'Name', 'MinOrderValue', 'DiscountPercent', 'MaxDiscount', 'IsActive'], 'pks': ['Id'], 'fks': []},
        'Vouchers': {'pos': (22, 15), 'cols': ['Id', 'Code', 'Type', 'Value', 'ExpiryDate', 'MaxUsages', 'UsedCount', 'CreatedAt'], 'pks': ['Id'], 'fks': []},
        'Notifications': {'pos': (22, 6), 'cols': ['Id', 'UserId', 'Message', 'IsRead', 'CreatedAt'], 'pks': ['Id'], 'fks': ['UserId']},
        'ChatMessages': {'pos': (22, 10), 'cols': ['Id', 'SenderId', 'ReceiverId', 'Message', 'IsRead', 'IsAdminSender', 'CreatedAt'], 'pks': ['Id'], 'fks': ['SenderId', 'ReceiverId']},
        'Drivers': {'pos': (18, 1), 'cols': ['Id', 'Name', 'Description', 'DownloadUrl', 'Brand'], 'pks': ['Id'], 'fks': []},
        'OtpCodes': {'pos': (2, 1), 'cols': ['Id', 'Email', 'Code', 'ExpiresAt', 'IsUsed', 'CreatedAt'], 'pks': ['Id'], 'fks': []},
        'RefreshTokens': {'pos': (10, 20), 'cols': ['Id', 'Token', 'UserId', 'ExpiresAt', 'IsRevoked', 'CreatedAt'], 'pks': ['Id'], 'fks': ['UserId']}
    }
    
    meta = {}
    for name, data in tables.items():
        meta[name] = draw_table(ax, data['pos'][0], data['pos'][1], name, data['cols'], data['pks'], data['fks'])
        
    # Draw connections (Manual selection of important relations)
    def connect(t1, t2, label1="1", label2="N"):
        p1 = meta[t1] # (x_center, y_center, w, h)
        p2 = meta[t2]
        ax.annotate("", xy=(p2[0], p2[1]), xytext=(p1[0], p1[1]), arrowprops=dict(arrowstyle="-", color='gray', alpha=0.5))
        # Add cardinality (simplified)
        ax.text(p1[0] + 0.2, p1[1] + 0.2, label1, color='blue', fontproperties=prop, fontsize=9)
        ax.text(p2[0] - 0.2, p2[1] + 0.2, label2, color='blue', fontproperties=prop, fontsize=9)

    connect('Users', 'Orders')
    connect('Users', 'CartItems')
    connect('Users', 'WishlistItems')
    connect('Users', 'Reviews')
    connect('Users', 'Notifications')
    connect('Users', 'RefreshTokens')
    connect('Users', 'ChatMessages')
    
    connect('Products', 'OrderItems')
    connect('Products', 'CartItems')
    connect('Products', 'WishlistItems')
    connect('Products', 'Reviews')
    
    connect('Orders', 'OrderItems')
    
    plt.title("GamingGear - Sơ đồ lược đồ cơ sở dữ liệu (ERD)", fontproperties=bold_prop, fontsize=20, pad=20)
    plt.tight_layout()
    output_path = r'd:\Hà Quang Huy_231A290063_WebNC\ERD_Diagram.png'
    plt.savefig(output_path, dpi=150, bbox_inches='tight')
    print(f"Success: {output_path}")

if __name__ == "__main__":
    main()
