   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   I EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:DragonDrop.WPF;assembly=DragonDrop.WPFDragonDrop.WPF 5   	   M IDragonDrop.Scanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:DragonDrop.ScannerDragonDrop.Scanner 5   	   #   DragonDrop.Scanner.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   Dbeh4clr-namespace:DragonDrop.WPF;assembly=DragonDrop.WPF     5   	   2local clr-namespace:DragonDrop.Scanner     5   	     � Title$  Scanner��5
   	    � SizeToContent$ Height=�6   $	��800��61    � WindowStartupLocation$ CenterScreen=�6=    � 
ResizeMode$ NoResize=�6b    � 	Resources 5         EntryBoxStyle �� 
TargetType �� Property �� Value  GrayLabelStyle��%o  &	        '�� �    '�� H    &	 _    �� 5   
   " ��6&   .�5      ��  � ��$ 1��6,   �� 5       � ��
 �z���6+   �� 5       � ��
 ��=H�6+   �� 5       � ��$ Center=�64   �� 5       �@��TextAlignment$ Center=�6.   �� 5       � ��$ Center=�62   �� 5       � ��$ 32(�6)   �� 5       � ��$ 10q�6(   �� 5       � ��$	 280��6&   5      �� 5   
   " ��6   .�5      ��  � ��$ 32(�6)   �� 5       � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       � ��$ Consolas)�6+   5      �� 5   
   " ��6   .�5      �� 
 �@��X1$ 0��6#   �� 5      
 �@��X2$
 1000��6#   �� 5        �@��StrokeDashArray$ 4R�60   �� 5!       � ��$ 2��60   �� 5"       � ��
 �����6'   �� 5#       � ��$ Center=�62   5$      �� 5%   
   " ��6'   .�5&      ��  � ��$ Center=�62   �� 5'       � ��$ Segoe UI)�6+   �� 5(       � ��$ 20(�6)   �� 5)       � ��
 �����6+   �� 5*       � ��$ Italic$�6*   5+      5,      .��5-      �� .+�5/   
   �� 
����=H� 50      ��%q  '��       �� 51      " ��6   .�52      ��  � ��$	 400��6.   �� 53       � ��$ Center=�6<   �� 54       � ��$ Center=�6:   �� 55       � ��$ 26(�61   �� 56       � ��
 �����63   �� 57       � ��
 �����63   �� 58       � ��$ Center=�6C   �� 59       �@�Tag$ SelectableTxtBox��6,   5:      5;      .+�5<      ��  �� Orientation$ Vertical=�6   �� 5=      $ 
Horizontal=�6   .+�5>      �� $��Host: ��6   
�������6)   �� 5?      ��6   �� *HostBoxText+5@      �� 5A      $ 
Horizontal=�6   .+�5B      �� $��Port: ��6   
�������6)   �� 5C      -   5        ��5C      �� *PortBoxText+67   5D      �� 5E      $ 
Horizontal=�6   $��Center=�62   .+�5F      �� $��Status: ��6   $��22(�6+   
�������69   �� 5G      $��26(�6:   
�������$��0q�6[   $��0q�6f   ��6   �� *StatusLblText+6f   �� 5H      $��30 0 0 0q�6   $��4q�6A   ��6+   Z� .��5I      �� $ 
Horizontal=�6!   .+�5J      �� $��/Art/whiteSynchro.png��6    $��36��6?   �� 5K      
�������$��20(�67   $��Refresh��6E   5L      .   (System.Windows.Interactivity.Interaction	   	Behaviors	 5M      +  %DragonDrop.WPF.EventToCommandBehavior  5N      
   Event$
 Click��5O   5      PassArguments .     Command 5N   5   �� *RetryConnectionCommand+5O   C   5Q      5R      5S      �� 5U   
   $ 
Horizontal=�6   $��Center=�6.   $��0 10 0 0q�6K   .+�5V      �� $��/Art/codeScanner.png��6   $��64��62   �� 5W      $��Mock Scanner��6   $
��Bold"�6+   5X      �� 5Y   
   $��0 10 0 0q�6   � 5[   
   $
��0 10q�6    � ColumnDefinitions 5\      �� 5]      $��* �6#   �� 5^      $
��auto �6#   �� 5_      $��* �6#    � RowDefinitions 5a      �� 5b      $��* �6    �� 5c      $��* �6    .+�5e      �� $��Enter a UPC code to send��6   #��[ 67   $
��Left=�5f      �� 5g      -   5        #��[  5g      ��6=   �� *CodeBoxText+.��5h   B   	 5i        5j      $
 TextChanged��5k   1    .  5j   1   �� *CodeBoxTextChangedCommand+5k   E   .��5l   +   5m      �� 5n      $��1��6   $��Wrong Format!��6!   $��3��69   
���<��y�5o      �� *FormatErrorVis+�� 5p      $��2��6   $��Right=�6%   $��2q�6W   $��0 0 6 0q�6k   ��6A   Z� .��5q      �� $ 
Horizontal=�6   .+�5r      �� $��/Art/dice.png��6   $��48��63   $��0q�6?   �� 5s      $��Generate��6   5t      	 5u        5v      $
 Click��6X    .   61   �� *GenRandCodeCommand+5w   1   5y      5z      � 5|   
    5}      �� 5~      $��* �6#   �� 5      $
��auto �6#   �� 5�      $��* �6#   .+�5�      �� �� 5�      $��1��6   $��or��6$   #��[ 61   $��Center=�6Y   $��28(�5�      �� 5�      $��2��6   5�      � 5�   
   $
��0 10q�6    5�      �� 5�      $��* �6#   �� 5�      $
��auto �6#   �� 5�      $��* �6#   .+�5�      �� $��Pick from Products:��6   #��[ 62   $
��Left=�5�      �� 5�      $��1��6   $��Stretch=�6'   $	��400��6E    �� DisplayMemberPath$
 Name��5�      $��Center=�60   $��22(�6T    �� IsTextSearchEnabled .  �� 
IsEditable . ��5�      �� *ProductsList+ �� SelectedItem 5�      �� *SelectedProduct+ �� Mode$ OneWayToSource=� �� UpdateSourceTrigger$ PropertyChanged=�	 5�        5�      $
 SelectionChanged��6W    .   61   �� *ProdSelectCommand+5�   1   5�      5�      �� 5�   
   $��Center=�6   $��10q�6.   $
��0 16q�6;   {�6I   �� *SubmitEnabled+.��5�      �� $ 
Horizontal=�6   .+�5�      �� $��/Art/sendArrow.png��6   $��48��64   �� 5�      $��	Send Code��6   5�      	 5�        5�      $
 Click��6O    .  6-   �� *SubmitCommand+6]   5�      5�      5�      