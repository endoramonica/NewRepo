// add autoDBmigrations vao file programs trong .api 


#27 Khởi tạo và điều hướng 
- Tạo trang chứa logic khởi tạo và điều hướng 

		Kiểm tra FirstRunKey từ Preferences

		Nếu là lần đầu → điều hướng Onboarding

		Nếu không, kiểm tra đăng nhập

		Nếu đã đăng nhập → điều hướng Home

		Nếu chưa → điều hướng Login


suy nghĩ đến việc tạo thêm một bottom sheet trong đó chứa thoogn tin về điều khoản và nút " tôi đồng ý và tiếp tục "
 
 
 //homepage.xmlns 
 <ScrollView>
            <VerticalStackLayout Spacing="10" Padding="10">

                <!-- Khung chứa bài viết 1 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
 HeightRequest="40"
 WidthRequest="40"
 StrokeShape="RoundRectangle 20"
 BackgroundColor="Aqua">
                                <Image Source="logo.png"
    WidthRequest="30"
    HeightRequest="30"
    VerticalOptions="Center"
    HorizontalOptions="Center"
    Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
    Text="Abhay Prince"
    VerticalOptions="Center" />

                                <Label FontSize="12"
    Text="04 Jan 2025"
    VerticalOptions="Center"
    TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="DeepPink" StrokeThickness="0">
                            <Image Aspect="AspectFill"
HeightRequest="250"
Source="bird.png"
HorizontalOptions="Fill"
VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
Text="Some random post text will go here"
Padding="10,5"
FontSize="14"
TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>

                </Border>

                <!-- Khung chứa bài viết 2 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
              HeightRequest="40"
              WidthRequest="40"
              StrokeShape="RoundRectangle 20"
              BackgroundColor="Aqua">
                                <Image Source="logo.png"
                 WidthRequest="30"
                 HeightRequest="30"
                 VerticalOptions="Center"
                 HorizontalOptions="Center"
                 Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
                 Text="Abhay Prince"
                 VerticalOptions="Center" />

                                <Label FontSize="12"
                 Text="04 Jan 2025"
                 VerticalOptions="Center"
                 TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="LightSalmon" StrokeThickness="0">
                            <Image Aspect="AspectFill"
             HeightRequest="250"
             Source="bird.png"
             HorizontalOptions="Fill"
             VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
             Text="Some random post text will go here"
             Padding="10,5"
             FontSize="14"
             TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>
                    
                </Border>

                <!-- Khung chứa bài viết 3 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
 HeightRequest="40"
 WidthRequest="40"
 StrokeShape="RoundRectangle 20"
 BackgroundColor="Aqua">
                                <Image Source="logo.png"
    WidthRequest="30"
    HeightRequest="30"
    VerticalOptions="Center"
    HorizontalOptions="Center"
    Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
    Text="Abhay Prince"
    VerticalOptions="Center" />

                                <Label FontSize="12"
    Text="04 Jan 2025"
    VerticalOptions="Center"
    TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="LawnGreen" StrokeThickness="0">
                            <Image Aspect="AspectFill"
HeightRequest="250"
Source="bird.png"
HorizontalOptions="Fill"
VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
Text="Some random post text will go here"
Padding="10,5"
FontSize="14"
TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>

                </Border>

                <!-- Khung chứa bài viết 4 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
 HeightRequest="40"
 WidthRequest="40"
 StrokeShape="RoundRectangle 20"
 BackgroundColor="Aqua">
                                <Image Source="logo.png"
    WidthRequest="30"
    HeightRequest="30"
    VerticalOptions="Center"
    HorizontalOptions="Center"
    Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
    Text="Abhay Prince"
    VerticalOptions="Center" />

                                <Label FontSize="12"
    Text="04 Jan 2025"
    VerticalOptions="Center"
    TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="DeepPink" StrokeThickness="0">
                            <Image Aspect="AspectFill"
HeightRequest="250"
Source="bird.png"
HorizontalOptions="Fill"
VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
Text="Some random post text will go here"
Padding="10,5"
FontSize="14"
TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>

                </Border>

                <!-- Khung chứa bài viết 5 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
              HeightRequest="40"
              WidthRequest="40"
              StrokeShape="RoundRectangle 20"
              BackgroundColor="Aqua">
                                <Image Source="logo.png"
                 WidthRequest="30"
                 HeightRequest="30"
                 VerticalOptions="Center"
                 HorizontalOptions="Center"
                 Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
                 Text="Abhay Prince"
                 VerticalOptions="Center" />

                                <Label FontSize="12"
                 Text="04 Jan 2025"
                 VerticalOptions="Center"
                 TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="LightSalmon" StrokeThickness="0">
                            <Image Aspect="AspectFill"
             HeightRequest="250"
             Source="bird.png"
             HorizontalOptions="Fill"
             VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
             Text="Some random post text will go here"
             Padding="10,5"
             FontSize="14"
             TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>

                </Border>

                <!-- Khung chứa bài viết 6 -->
                <Border Padding="8" StrokeThickness="0" BackgroundColor="White">
                    <Grid RowDefinitions="Auto, Auto, Auto, Auto,Auto" RowSpacing="8">

                        <!-- Phần thông tin người đăng -->
                        <Grid ColumnDefinitions="40, Auto, *" ColumnSpacing="5" Padding="5">

                            <!-- Avatar -->
                            <Border Grid.Column="0"
 HeightRequest="40"
 WidthRequest="40"
 StrokeShape="RoundRectangle 20"
 BackgroundColor="Aqua">
                                <Image Source="logo.png"
    WidthRequest="30"
    HeightRequest="30"
    VerticalOptions="Center"
    HorizontalOptions="Center"
    Aspect="AspectFit" />
                            </Border>

                            <!-- Thông tin người dùng -->
                            <VerticalStackLayout Grid.Column="1" Spacing="2">
                                <Label FontAttributes="Bold"
    Text="Abhay Prince"
    VerticalOptions="Center" />

                                <Label FontSize="12"
    Text="04 Jan 2025"
    VerticalOptions="Center"
    TextColor="Gray" />
                            </VerticalStackLayout>
                        </Grid>

                        <!-- Hình ảnh bài viết -->
                        <Border Grid.Row="1" BackgroundColor="LawnGreen" StrokeThickness="0">
                            <Image Aspect="AspectFill"
HeightRequest="250"
Source="bird.png"
HorizontalOptions="Fill"
VerticalOptions="Fill" />
                        </Border>

                        <!-- Nội dung bài viết -->
                        <Label Grid.Row="2"
Text="Some random post text will go here"
Padding="10,5"
FontSize="14"
TextColor="Black" />

                        <!-- Đường kẻ mờ -->
                        <BoxView Grid.Row="3" Style="{StaticResource FadedLineStyle}" />

                        <!-- Gọi lại PostActionButtons -->
                        <controls:PostActionButtons Grid.Row="4" Padding="5" />

                    </Grid>

                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
                    </Border.GestureRecognizers>

                </Border>


            </VerticalStackLayout>
        </ScrollView>