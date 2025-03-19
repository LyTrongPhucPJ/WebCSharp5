console.log("📂 staff.js đã được tải!");

let cart = [];
let totalAmount = 0;
let discountPercentage = 0;

document.addEventListener('DOMContentLoaded', () => {
    initializeFilters();
    checkCartStatus();
});
function printInvoice() {
    generatePDF(); // Gọi lại hàm xuất hóa đơn
    let paymentSuccessModal = bootstrap.Modal.getInstance(document.getElementById('paymentSuccessModal'));
    paymentSuccessModal.hide(); // Đóng modal sau khi in
}

function initializeFilters() {
    document.querySelectorAll('.filter-button button').forEach(button => {
        button.addEventListener('click', function () {
            document.querySelector('.filter-button button.active')?.classList.remove('active');
            this.classList.add('active');
            let categoryId = this.getAttribute('data-category');
            filterProducts(categoryId === 'all' ? 'Tất cả' : categoryId);
        });
    });
    document.getElementById('discount').addEventListener('change', function () {
        discountPercentage = parseInt(this.value);
        updateCart();
    });
}

function filterProducts(categoryId) {
    console.log(`Filtering by category: ${categoryId}`);
    const productsSection = document.getElementById('productsSection');
    const products = document.querySelectorAll('.product');
    let hasVisibleProducts = false;

    products.forEach(product => {
        const productCategoryId = product.getAttribute('data-category');
        console.log(`Product: ${product.querySelector('.product-name').textContent}, Category: ${productCategoryId}`);
        if (categoryId === "Tất cả" || productCategoryId === categoryId) {
            product.style.display = '';
            product.style.height = 'auto';
            hasVisibleProducts = true;
        } else {
            product.style.display = 'none';
        }
    });

    productsSection.style.display = 'grid';
    productsSection.style.gap = '13px';
    productsSection.style.maxWidth = '100%';
    productsSection.style.width = '100%';

    const noProductsMessage = productsSection.querySelector('.no-products');
    if (!hasVisibleProducts && !noProductsMessage) {
        const message = document.createElement('p');
        message.textContent = 'Không có sản phẩm nào trong danh mục này.';
        message.className = 'no-products';
        productsSection.appendChild(message);
    } else if (hasVisibleProducts && noProductsMessage) {
        noProductsMessage.remove();
    }
}

function searchProducts() {
    const searchTerm = document.getElementById('search').value.trim().toLowerCase();
    const products = document.querySelectorAll('.product');
    let hasVisibleProducts = false;
    const productsSection = document.getElementById('productsSection');

    if (searchTerm === '') {
        products.forEach(product => {
            product.style.display = '';
            product.style.height = 'auto';
        });
        productsSection.style.gap = '13px';
        return;
    }

    productsSection.style.display = 'grid';
    productsSection.style.gap = '13px';
    productsSection.style.maxWidth = '100%';
    productsSection.style.width = '100%';

    products.forEach(product => {
        const productName = product.getAttribute('data-name').toLowerCase().trim();

        if (normalizeString(productName).includes(normalizeString(searchTerm))) {
            product.style.display = '';
            product.style.height = 'auto';

            hasVisibleProducts = true;
        } else {
            product.style.display = 'none';
        }
    });

    const noProductsMessage = productsSection.querySelector('.no-products');
    if (!hasVisibleProducts && searchTerm !== '') {
        if (!noProductsMessage) {
            const message = document.createElement('p');
            message.textContent = 'Không tìm thấy sản phẩm nào.';
            message.className = 'no-products';
            productsSection.appendChild(message);
        }
    } else if (hasVisibleProducts && noProductsMessage) {
        noProductsMessage.remove();
    }

    console.log(`Search term: "${searchTerm}", Visible products: ${hasVisibleProducts}`);
}

// Hàm chuẩn hóa để loại bỏ dấu và chuyển sang chữ thường
function normalizeString(str) {
    return str
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .toLowerCase();
}


function addToCart(productName, productPrice, productImg) {
    let existingProduct = cart.find(product => product.name === productName);

    if (existingProduct) {
        existingProduct.quantity++;
    } else {

        cart.push({ name: productName, price: productPrice, img: productImg, quantity: 1 });
    }

    updateCart();
    checkCartStatus();
}
function checkCartStatus() {
    const isCartEmpty = cart.length === 0;
    const elements = [
        'confirm-btn',
        'export-btn',
        'discount',
        'customer-money',
        'note',
        'customer-id'
    ];
    elements.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.disabled = isCartEmpty;
        } else {
            console.error(`Không tìm thấy phần tử với ID: ${id}`);
        }
    });
}
function updateCart() {
    console.log('Updating cart:', cart);
    let orderTableBody = document.querySelector('#orderTableBody');
    orderTableBody.innerHTML = '';
    totalAmount = 0;

    cart.forEach(product => {
        let row = document.createElement('tr');
        row.innerHTML = `
        <td class="product-name-td">${product.name}</td>
        <td>${formatNumber(product.price)}<sup>đ</sup></td>
        <td class="quantity-container">
            <button class="quantity-btn-subtract" onclick="updateQuantity('${product.name}', -1)">-</button>
            <span class="quantity-value">${product.quantity}</span> 
            <button class="quantity-btn" onclick="updateQuantity('${product.name}', 1)">+</button>
        </td>
        <td><button class="remove-btn" onclick="removeProduct('${product.name}')">X</button></td>
    `;
        orderTableBody.appendChild(row);
        totalAmount += product.price * product.quantity;
    });


    let discountedAmount = totalAmount - (totalAmount * discountPercentage / 100);
    document.getElementById('total-amount').textContent = formatNumber(discountedAmount) + " đ";
    calculateChange();

    checkCartStatus();
}

function updateQuantity(productName, delta) {
    let product = cart.find(p => p.name === productName);
    if (product) {
        product.quantity += delta;
        if (product.quantity <= 0) {
            removeProduct(productName);
        } else {
            updateCart();
        }
    }
}

function removeProduct(productName) {
    cart = cart.filter(product => product.name !== productName);
    updateCart();
}

function calculateChange() {
    let inputElement = document.getElementById('customer-money');
    let changeElement = document.getElementById('change');
    let errorMessageElement = document.getElementById('error-message');
    let confirmButton = document.getElementById('confirm-btn');

    // Lấy số tiền khách đưa và loại bỏ dấu chấm ngăn cách
    let customerMoney = parseInt(inputElement.value.replace(/\./g, '')) || 0;

    // Tính tổng tiền cần thanh toán sau khi giảm giá (đã nhân 1000 để đúng đơn vị)
    let discountedAmount = totalAmount * (1 - discountPercentage / 100) * 1000;

    if (customerMoney < discountedAmount) {
        // Nếu tiền khách đưa không đủ, hiển thị cảnh báo
        changeElement.value = '';
        errorMessageElement.style.display = 'block';
        errorMessageElement.textContent = "❌ Số tiền không đủ";
        confirmButton.disabled = true;
    } else {
        // Nếu đủ tiền, tính tiền thừa và hiển thị
        let change = customerMoney - discountedAmount;
        changeElement.value = change.toLocaleString('vi-VN') + " đ";
        errorMessageElement.style.display = 'none';
        confirmButton.disabled = false;
    }
}
function formatMoney(input) {
    // Loại bỏ tất cả dấu chấm và ký tự không phải số
    let value = input.value.replace(/\./g, '').replace(/\D/g, '');

    if (value.length > 0) {
        let numericValue = parseInt(value, 10); // Chuyển thành số nguyên

        // Nếu chỉ có 1 chữ số đầu tiên, hiển thị ".000"
        if (value.length === 1) {
            input.value = numericValue.toLocaleString('vi-VN') + ".000";
        } else {
            input.value = numericValue.toLocaleString('vi-VN'); // Không thêm ".000" nữa
        }

        // Lưu giá trị thực tế (chia cho 100 để tính toán đúng)
        input.dataset.actualValue = (numericValue / 1000).toFixed(0);
    } else {
        input.dataset.actualValue = 0;
        input.value = ''; // Khi xóa hết, không hiển thị gì
    }

    calculateChange(); // Cập nhật tiền thừa
}

function handlePayment() {
    const customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '')) || 0;
    const discountedAmount = totalAmount * 1000 * (1 - discountPercentage / 100);

    if (customerMoney === 0) {
        alert('Vui lòng nhập số tiền khách đưa.');
    } else if (customerMoney < discountedAmount) {
        alert('Số tiền không đủ để thanh toán.');
    } else {
        // Hiển thị modal thay vì alert
        let paymentSuccessModal = new bootstrap.Modal(document.getElementById('paymentSuccessModal'));
        paymentSuccessModal.show();
        // Đặt lại đơn hàng sau 2 giây
        setTimeout(resetOrder, 2000);
    }
}


function resetOrder() {
    // Xóa giỏ hàng
    cart = [];
    totalAmount = 0;
    discountPercentage = 0; // Reset giảm giá về 0%

    // Xóa sản phẩm trong bảng
    let orderTableBody = document.getElementById('orderTableBody');
    if (orderTableBody) {
        orderTableBody.innerHTML = ''; // Xóa hết sản phẩm trong bảng
    }

    // Reset lại các giá trị input
    document.getElementById('customer-money').value = '';
    document.getElementById('change').value = '';
    document.getElementById('total-amount').textContent = '0 đ';
    document.getElementById('error-message').style.display = 'none';
    document.getElementById('note').value = '';
    document.getElementById('customer-id').value = '';
    document.getElementById('customer-money').dataset.actualValue = "0";

    // ⚠️ Đảm bảo giảm giá về "Không có" (giá trị là '0')
    let discountElement = document.getElementById('discount');
    if (discountElement) {
        discountElement.value = '0'; // Đảm bảo UI hiển thị đúng
        discountElement.dispatchEvent(new Event('change')); // Kích hoạt sự kiện change để cập nhật lại giá trị
    }

    // Cập nhật giao diện lại
    updateCart();
    console.log("✅ Reset đơn hàng thành công! Giỏ hàng & giảm giá đã về mặc định.");
}


function toggleDropdown() {
    const dropdown = document.getElementById('dropdownMenu');
    dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
}

function formatNumber(number) {
    return number.toFixed(3).replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function exportInvoice() {
    if (cart.length === 0) {
        alert('Không có đơn hàng để xuất hóa đơn.');
        return;
    }

    const customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '').replace(',', '')) || 0;
    if (isNaN(customerMoney) || customerMoney === 0) {
        alert('Vui lòng nhập số tiền khách đưa.');
        return;
    }

    generatePDF();
}

function generatePDF() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({
        orientation: 'portrait',
        unit: 'mm',
        format: 'a4'
    });

    // Cấu hình font, màu sắc
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(12);
    const primaryColor = '#006F3C';
    const textColor = '#333';

    // Tiêu đề hóa đơn
    doc.setFontSize(20);
    doc.setTextColor(primaryColor);
    doc.setFont('helvetica', 'bold');
    doc.text('HÓA ĐƠN', 105, 20, { align: 'center' });

    // Thông tin cửa hàng
    doc.setFontSize(10);
    doc.setTextColor(textColor);
    doc.setFont('helvetica', 'normal');
    doc.text('Cửa hàng: Lion Store', 20, 40);
    doc.text('Địa chỉ: Hà Nội, Việt Nam', 20, 50);
    doc.text('SĐT: 0987654321', 20, 60);

    // Thông tin khách hàng
    doc.setFontSize(12);
    doc.text(`Khách hàng: Nguyễn Quốc Anh`, 20, 80);
    doc.text(`Ngày xuất: ${new Date().toLocaleString()}`, 20, 90);

    // Bảng sản phẩm
    let yPosition = 110;
    doc.setFont('helvetica', 'bold');
    doc.text('Sản phẩm', 20, yPosition);
    doc.text('Giá', 100, yPosition, { align: 'right' });
    doc.text('SL', 130, yPosition, { align: 'center' });
    doc.text('TT', 160, yPosition, { align: 'right' });
    yPosition += 5;

    doc.setDrawColor(200, 200, 200);
    doc.setLineWidth(0.2);
    doc.line(15, yPosition, 195, yPosition);
    yPosition += 5;

    cart.forEach(product => {
        const productPriceVND = product.price * 1000; // Chuyển từ đơn vị DB sang VND
        const productTotal = productPriceVND * product.quantity;

        doc.text(product.name, 20, yPosition);
        doc.text(formatCurrency(productPriceVND), 100, yPosition, { align: 'right' });
        doc.text(product.quantity.toString(), 130, yPosition, { align: 'center' });
        doc.text(formatCurrency(productTotal), 160, yPosition, { align: 'right' });
        yPosition += 10;

        doc.line(15, yPosition, 195, yPosition);
    });

    // Tổng tiền hiển thị đúng
    doc.setFont('helvetica', 'bold');
    const totalAmountWithDiscount = totalAmount * 1000 * (1 - discountPercentage / 100); // Nhân 1000 để thành VND

    doc.text(`Tổng cộng: ${formatCurrency(totalAmountWithDiscount)}`, 160, yPosition + 5, { align: 'right' });
    yPosition += 10;

    // Lấy số tiền khách đưa từ input
    let customerMoneyInput = document.getElementById('customer-money').value.replace(/\./g, '').replace(',', '');
    let customerMoney = parseFloat(customerMoneyInput) || 0;

    // Nếu khách nhập "29", thực tế là 29.000 VND => Nhân 1000 để trừ chính xác
    const change = (customerMoney) - totalAmountWithDiscount;

    // Debug log để kiểm tra dữ liệu
    console.log("Tiền khách đưa (VND):", customerMoney * 1000);
    console.log("Tổng tiền cần thanh toán (VND):", totalAmountWithDiscount);
    console.log("Tiền thừa (VND):", change);

    doc.text(`Khách đưa: ${formatCurrency(customerMoney)}`, 160, yPosition + 5, { align: 'right' });
    yPosition += 10;
    doc.text(`Tiền thừa: ${formatCurrency(change)}`, 160, yPosition + 5, { align: 'right' });

    // Cảm ơn khách hàng
    doc.setFontSize(10);
    doc.setTextColor(primaryColor);
    doc.text('Cảm ơn quý khách! Powered by Lion Store', 105, 280, { align: 'center' });
    doc.setTextColor(textColor);
    doc.text('Liên hệ: 0987654321 | www.lion.com', 105, 290, { align: 'center' });

    // Lưu file PDF
    doc.save('HoaDon_Receipt.pdf');
}


function viewInvoice() {
    let invoiceWindow = window.open('', '_blank');
    if (!invoiceWindow) {
        alert("Trình duyệt chặn popup! Vui lòng cho phép.");
        return;
    }

    let totalAmountWithDiscount = totalAmount * 1000 * (1 - discountPercentage / 100);
    let customerMoney = parseFloat(document.getElementById('customer-money').value.replace(/\./g, '')) || 0;
    let change = customerMoney - totalAmountWithDiscount;
    let totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    let invoiceNumber = "HD" + Math.floor(Math.random() * 1000000);

    let invoiceHTML = `
        <html>
        <head>
            <title>Hóa đơn</title>
            <style>
                body {
                    font-family: 'Courier New', monospace;
                    text-align: center;
                    background: white;
                    margin: 10px;
                }
                h2 {
                    font-size: 16px;
                    font-weight: bold;
                    text-transform: uppercase;
                }
                .header {
                    text-align: center;
                    font-size: 12px;
                    margin-bottom: 5px;
                }
                .table {
                    width: 15%;
                    margin: 10px auto;
                    border-collapse: collapse;
                    font-size: 12px;
                    text-align: center;
                }
                .table th, .table td {
                    border: 1px solid black; /* Viền đầy đủ */
                    padding: 5px;
                }
                .total-section {
                    font-size: 13px;
                    font-weight: bold;
                    margin-top: 5px;
                }
                .print-btn {
                    margin-top: 10px;
                    padding: 8px 12px;
                    font-size: 12px;
                    border: none;
                    background: #2EAE74;
                    color: white;
                    cursor: pointer;
                    border-radius: 4px;
                }
            </style>
        </head>
        <body>
            <div class="header">
                <h2>LION COFFEE</h2>
                <p>Hotline: 0347999999</p>
                <p>Pass WiFi: 66669999</p>
                <h2><strong>HÓA ĐƠN THANH TOÁN</strong></h2>
                <p><strong>${invoiceNumber} - Khách lẻ</strong></p>
                <p>${new Date().toLocaleString()}</p>
            </div>

            <table class="table">
                <tr>
                    <th style="text-align:left;">Tên món</th>
                    <th>SL</th>
                    <th>ĐG</th>
                    <th>Thành tiền</th>
                </tr>`;

    cart.forEach(product => {
        let productTotal = product.price * product.quantity * 1000;
        let formattedProductName = product.name.length > 25 ? product.name.slice(0, 25) + "..." : product.name;

        invoiceHTML += `
        <tr>
            <td style="text-align:left;">${formattedProductName}</td>
            <td>${product.quantity}</td>
            <td>${formatCurrency(product.price * 1000)}</td>
            <td>${formatCurrency(productTotal)}</td>
        </tr>`;
    });

    let discountAmount = totalAmount * 1000 * (discountPercentage / 100);

    invoiceHTML += `
        </table>

        <p class="total-section">Tổng tiền hàng: ${formatCurrency(totalAmount * 1000)}</p>
        <p class="total-section">Giảm giá: ${formatCurrency(discountAmount)}</p>
        <p class="total-section">Tổng thanh toán: ${formatCurrency(totalAmountWithDiscount)}</p>
        <p style="font-size: 12px">caphelionfpolycantho.id.vn/</p>
        <p style="font-size: 12px; font-weight: bold;">CẢM ƠN VÀ HẸN GẶP LẠI QUÝ KHÁCH!</p>

        <button class="print-btn" onclick="window.print()">In hóa đơn</button>
        </body>
        </html>`;

    invoiceWindow.document.write(invoiceHTML);
    invoiceWindow.document.close();
}



// Hàm format tiền tệ VNĐ
function formatCurrency(number) {
    return new Intl.NumberFormat('vi-VN').format(number) + " đ";
}

// Hàm format tiền tệ chuẩn Việt Nam
function formatCurrency(number) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(number);
}


// Hàm định dạng giá tiền
function formatPrice(price) {
    return price ? price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',') + 'đ' : "0đ";
}
function logout() {
    console.log("🛑 Gọi hàm logout()...");

    fetch('/User/Logout', { method: 'POST' })
        .then(response => {
            console.log("📡 Gửi yêu cầu Logout, trạng thái:", response.status);
            if (response.redirected) {
                console.log("✅ Chuyển hướng về:", response.url);
                window.location.href = response.url; // Chuyển về trang đăng nhập
            } else {
                console.error("❌ Không có chuyển hướng. Kiểm tra backend.");
            }
        })
        .catch(error => console.error("⚠️ Lỗi khi gọi API Logout:", error));
}
function getUserName() {
    fetch('/User/GetUserName') // Gọi API lấy tên user
        .then(response => {
            if (!response.ok) {
                throw new Error("Không thể lấy thông tin người dùng");
            }
            return response.json();
        })
        .then(data => {
            console.log("✅ User name:", data.userName);
            document.getElementById("userName").innerText = data.userName; // Cập nhật UI
        })
        .catch(error => console.error("⚠️ Lỗi lấy tên user:", error));
}

// Gọi hàm khi trang được tải
document.addEventListener("DOMContentLoaded", function () {
    getUserName();
});
