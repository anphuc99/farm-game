# Triển Khai Dự Án Game Nông Trại Trên Unity

## 1. Tổng Quan Kiến Trúc MVC và Hệ Thống Binding

### MVC (Model - View - Controller)
- **Model**:  
  - Chứa các lớp kế thừa từ lớp cơ sở *Model*, chịu trách nhiệm lưu trữ và truy xuất dữ liệu game.
  - Sử dụng **Collection** để truy vấn và cập nhật dữ liệu thông qua các phương thức như `LoadModel` và `SaveModel`.

- **Controller**:  
  - Các lớp static chứa logic game:
    - **Trồng cây, thu hoạch, phá hủy cây.**
    - **Kiểm tra trạng thái cây** (còn non, đã phát triển, đã chết) và lấy thông tin liên quan: thời gian phát triển, số lượng trái, thời gian cho trái mới, thời gian cây chết.
    - **Quản lý trạng thái của Worker**: cập nhật trạng thái (rảnh, đang trồng, thu hoạch, dọn dẹp cây chết) theo thời gian thực, kể cả khi game ở chế độ offline.

- **View**:  
  - Sử dụng UnityEngine để hiển thị giao diện và thông tin game.
  - Liên kết với Model thông qua hệ thống binding được xây dựng bởi các lớp **Bindable** và **BindableList**.
  - Tự động cập nhật giao diện khi Model thay đổi dữ liệu nhờ hệ thống binding.

---

## 2. Các Module Hỗ Trợ và Logic Nền Tảng

### DataHelper
- Sử dụng **Collection** để lưu trữ dữ liệu từ Model.
- Hỗ trợ lưu dữ liệu sang file và mã hóa dữ liệu, đảm bảo an toàn thông tin cho game.

### DateTimeHelper
- Cung cấp các phương thức xử lý liên quan đến thời gian, phục vụ cho việc tính toán thời gian phát triển cây, cập nhật trạng thái Worker, và các tác vụ thời gian khác.

### Hệ Thống Binding
- **Bindable** và **BindableList**:  
  - Đảm nhiệm việc liên kết dữ liệu từ Model sang View.
  - Tự động thông báo và cập nhật giao diện khi dữ liệu trong Model thay đổi.

### Anti-Cheat
- **BindableAntiCheat**:  
  - Cung cấp các biện pháp chống cheat cơ bản nhằm ngăn chặn hành vi gian lận của người dùng.

---

## 3. Cấu Trúc Các Thành Phần Chính

### Model
- **Vai trò**:
  - Lưu trữ và truy xuất dữ liệu game.
  - Các lớp kế thừa từ lớp *Model* sử dụng **Collection** với các phương thức `LoadModel` và `SaveModel` để thao tác với dữ liệu.

### Controller
- **Vai trò**:
  - Chứa logic game dưới dạng các static class.
  - Xử lý các nghiệp vụ chính như:
    - Trồng cây, thu hoạch, phá hủy cây.
    - Kiểm tra trạng thái cây (từ non đến trưởng thành hoặc chết) và lấy thông tin như thời gian phát triển, số lượng trái, thời gian cho trái mới, thời gian chết.
    - Quản lý trạng thái của Worker: cập nhật hành động (rảnh, đang trồng, thu hoạch, dọn dẹp cây chết) theo thời gian thực, kể cả chế độ offline.

### View
- **Vai trò**:
  - Hiển thị giao diện và thông tin game dựa trên dữ liệu từ Controller.
  - Tương tác với UnityEngine để cập nhật UI.
  - Kết nối với Model qua hệ thống binding (**Bindable** và **BindableList**) để tự động cập nhật khi có thay đổi.
  - Tích hợp cơ chế chống cheat thông qua **BindableAntiCheat** để bảo vệ game khỏi hành vi gian lận.

### Master Data
- **Vai trò**:
  - Lưu trữ dữ liệu không thay đổi (config data) của game.
  - Cho phép Game Designer cấu hình dữ liệu thông qua Google Sheet.
  - Tích hợp menu:
    - **Game Fram/Google Sheet/Open Google Sheet**: mở file cấu hình.
    - **Game Fram/Google Sheet/Load Data From Google Sheet**: tải dữ liệu cấu hình về dự án.

---

## 4. Cấu Trúc Thư Mục Dự Án

- **Thư mục chính**: `Assets/_GamePlay`  
  - Chứa toàn bộ asset của dự án (scripts, art texture, …).

- **Các thư mục bên trong**:
  - **Animation**: Chứa các file animation cho nhân vật và đối tượng trong game.
  - **Art**: Chứa texture, sprite và các asset đồ họa khác.
  - **Editor**: Chứa các script dành riêng cho công cụ Editor của Unity.
  - **Prefabs**: Chứa các prefab được sử dụng nhiều lần trong game.
  - **Resources**: Chứa các tài nguyên được load động trong game.
  - **Scenes**: Chứa các scene của game (ví dụ: StartScene, GamePlay).
  - **Scripts**: Chứa toàn bộ mã nguồn của dự án, được chia thành các thư mục con:
    - **Controllers**: Các class Controller chứa logic game.
    - **MasterData**: Chứa các class và dữ liệu cấu hình không thay đổi.
    - **Models**: Các class Model quản lý dữ liệu game, sử dụng **Collection** cho việc truy xuất và lưu trữ.
    - **Modules**: Chứa các module hỗ trợ như *DataHelper*, *DateTimeHelper*, và hệ thống binding (*Bindable*, *BindableList*, *BindableAntiCheat*).
    - **UnitTest**: Chứa các bài test để kiểm thử logic game và các Controller.
    - **Views**: Chứa các class hiển thị giao diện, liên kết với Controller và Model thông qua hệ thống binding.

---

## 5. Quy Trình Triển Khai Dự Án

1. **Khởi tạo dự án và thiết lập cấu trúc thư mục**:
   - Tạo project Unity mới.
   - Thiết lập thư mục `Assets/_GamePlay` cùng với các thư mục con (Animation, Art, Editor, Prefabs, Resources, Scenes, Scripts).

2. **Xây dựng hệ thống Model, Controller và View**:
   - Phát triển lớp Model cơ sở và các lớp con sử dụng **Collection** để truy xuất và lưu trữ dữ liệu.
   - Xây dựng các static class Controller để xử lý logic game, tích hợp các module hỗ trợ như *DataHelper* (với khả năng lưu file và mã hóa dữ liệu) và *DateTimeHelper*.
   - Phát triển View, tích hợp hệ thống binding (**Bindable**, **BindableList**) để tự động cập nhật giao diện khi Model thay đổi.
   - Triển khai **BindableAntiCheat** để bảo vệ game khỏi các hành vi gian lận của người chơi.

3. **Tích hợp Master Data và cấu hình từ Google Sheet**:
   - Thiết lập menu trong Game Fram để mở và tải dữ liệu cấu hình từ Google Sheet.
   - Đảm bảo dữ liệu cấu hình được load chính xác và không bị thay đổi trong quá trình chơi.

4. **Kiểm thử và tối ưu hóa**:
   - Viết Unit Test trong thư mục UnitTest để kiểm thử logic của Controller và các module hỗ trợ.
   - Kiểm thử hệ thống binding để đảm bảo View cập nhật tự động khi Model thay đổi.
   - Tối ưu hiệu suất game cho các nền tảng mục tiêu (desktop, mobile, …).

5. **Phát hành và bảo trì**:
   - Sau khi hoàn thiện và kiểm thử, đóng gói game và xuất bản trên các nền tảng mục tiêu.
   - Theo dõi phản hồi của người chơi để cập nhật, bảo trì và phát triển các tính năng mới, cũng như nâng cao hệ thống chống cheat.
