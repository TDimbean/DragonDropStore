   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	   I EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DragonDrop.WPFDragonDrop.WPF 5   	   I)clr-namespace:DragonDrop.WPF.MainSubViewsDragonDrop.WPF.MainSubViews 5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5	   	   .   (DragonDrop.WPF.MainSubViews.RegisterView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	   ;local)clr-namespace:DragonDrop.WPF.MainSubViews     5   	   5my&clr-namespace:System;assembly=mscorlib     5	   	   -   5          � Title$%  Register with Dragon Drop Store��5   	   $	��850��61   $	��500��6>    � WindowStartupLocation$ CenterScreen=�6J    � 	Resources 5       	  Title��%*   &	        ��5      Register��60   5      .��5      ]� .+�5   
   � $	��Top=�6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�62   $	��Top=�6M   $��8q�6e   .+�5      �� $��../Art/dragon.png��6   $��64��63   �� 5      #���   6   $��Center=�6;   $��36(�6V   $��10q�6d   $��Yu Gothic Ui Semibold)�6p   5      �� 5      -   5        
������ $��64��5   B   $��64��6M   $��Right=�6Y   $	��Top=�6u    � ToolTip$ Return(Esc)��6�   
������ .��5      �� $��../Art/close.png��6   5      5      �� 5   
   $ Vertical=�6    5!        EntryHeight  FieldTitle_Style �� 
TargetType �� Property �� Value  FieldEntry_Style  FieldEntryImmutables_Style  FieldEntryCheck_Style  FieldError_Style  SubmitBtn_Style  DashedLine_Style��%9  &	       &	      &	 �     7  &	 "    7  &	 �    7  7  &	 �    &	 �    &	 �    X�5"      30Q�64   �� 5#      " ��61   .�5$      ��  � ��$ Center=�6<   �� 5%       � ��$ 22(�61   5&      �� 5'      " ��61   .�5(      ��  � ��$ 50 0 0 0q�6/   �� 5)       � ��
 � ���63   �� 5*       � ��8   6/   �� 5+       � ��$	 400��6.   �� 5,       �@��TextAlignment$ Center=�66   �� 5-       � ��$ Center=�6A   �� 5.       � ��$ 20(�61   �� 5/       � ��$ Verdana)�63   �� 50       �@�Tag$ LosslessTxtBox��6,   51      �� 52      " ��6;   .�53      ��  � ��8   6/   �� 54       �@��TextAlignment$ Center=�66   �� 55       � ��$ 20(�61   �� 56       � ��$ Verdana)�63   57      �� 58      " ��66   .�59      ��  � ��$ ../Art/check.png��6/   �� 5:       � ��8   6/   �� 5;       � ��8  6.   �� 5<       � ��$ 0q�6/   �� 5=       � ��$
 Left=�6<   5>      �� 5?      " ��61   .�5@      ��  � ��
 �  ��63   �� 5A       � ��$ DemiBold"�63   �� 5B       � ��$ Center=�6<   �� 5C       � ��$ 18(�61   5D      �� 5E      " ��60   .�5F      ��  � ��$ 30(�61   �� 5G       � ��$ Verdana)�63   �� 5H       � ��$ DemiBold"�63   �� 5I       � ��$	 160��6.   �� 5J       � ��$ 10q�6/   �� 5K       � ��$
 0 10q�60   �� 5L       � ��
 ��� �64   �� 5M       � ��
 ���� 63   5N      �� 5O      " ��61   .�5P      ��  � ��
 �����6/   �� 5Q       � ��$ 1��6/   �� 5R       � ��$ 1��68   �� 5S       �@��StrokeDashArray$	 4 4R�68   �� 5T       �@J�SnapsToDevicePixels . 6<   5U      5V      .+�5Y      �� $ Vertical=�6   .+�5[      �� $
��Name��6   #��[ 5\      $3 -Name should be no longer than 100 characters.��5]      �� 5^      $ 
Horizontal=�6   .+�5_      �� -   5         ��Name$ NameBox��5_      #��[ 60   ��5`      �� 	 �� Path$	 NameBoxText�
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�.��6r   .   (System.Windows.Interactivity.Interaction   	Behaviors 5a      +  %DragonDrop.WPF.EventToCommandBehavior  5b         Event$ TextChanged��5d   !      PassArguments .     Command 5c   !   �� *NameTextChangedCommand+5e   !   .��5f   3   5g      �� 5h      #��[ 6   y�5i      �� $	 NameCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5l      �� 5m      #��[ 5o      ��5m      �� $	 NameErrorText�$
 TwoWay=�$ PropertyChanged=�y�5n      �� $	 NameErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5o      �� 5p      #��[ 6   5q      �� 5t      $ Vertical=�6   .+�5v      �� $��E-mail��6   #��[ 5w      $5 /Valid E-mail format. Limited to 100 characters.��5x      �� 5y      $ 
Horizontal=�6   .+�5z      �� -   5        $ EmailBox��5z      #��[ 60   ��5{      �� $	 EmailBoxText�$
 TwoWay=�$ PropertyChanged=�.��6s    5|        5}      $ TextChanged��5   !    .   5~   !   �� *EmailTextChangedCommand+5�   !   .��5�   3   5�      �� 5�      #��[ 6   y�5�      �� $	 EmailCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 5�      ��5�      �� $	 EmailErrorText�$
 TwoWay=�$ PropertyChanged=�y�5�      �� $	 EmailErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 6   5�      �� 5�      $ Vertical=�6   .+�5�      �� $��Phone��6   #��[ 5�      $ Format: ###-###-####��5�      �� 5�      $ 
Horizontal=�6   .+�5�      �� $ 
Horizontal=�6!   $��Center=�6:   .+�5�      �� -   5        $ PhoneBox��5�      #��[ 6�   ��60   �� $	 PhoneBoxText�$
 TwoWay=�$ PropertyChanged=�.��5�       5�        5�      $ TextChanged��5�   !    .  5�   !   �� *PhoneTextChangedCommand+5�   !   .��5�   3   5�      5�      �� 5�      #��[ 6   y�5�      �� $	 PhoneCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 5�      ��5�      �� $	 PhoneErrorText�$
 TwoWay=�$ PropertyChanged=�y�5�      �� $	 PhoneErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 6   5�      �� 5�      $ Vertical=�6   .+�5�      �� $��Address��6   #��[ 5�      $ Up to 200 characters.��5�      �� 5�      $ 
Horizontal=�6   .+�5�      �� -   5        $ AdrBox��5�      #��[ 6.   ��5�      �� $	 
AdrBoxText�$
 TwoWay=�$ PropertyChanged=�.��6p    5�        5�      $ TextChanged��5�   !    .   5�   !   �� *AdrTextChangedCommand+5�   !   .��5�   3   5�      �� 5�      #��[ 6   y�5�      �� $	 AdrCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 5�      ��5�      �� $	 AdrErrorText�$
 TwoWay=�$ PropertyChanged=�y�5�      �� $	 AdrErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 6   5�      �� 5�      $ Vertical=�6   .+�5�      �� $
��City��6   #��[ 5�      $  Limited to 100 characters.��5�      �� 5�      $ 
Horizontal=�6   .+�5�      �� -   5        $ CityBox��5�      #��[ 6/   ��5�      �� *CityBoxText+$
 TwoWay=�$ PropertyChanged=�.��6m    5�        5�      $ TextChanged��5�   !    .   5�   !   �� *CityTextChangedCommand+5�   !   .��5�   3   5�      �� 5�      #��[ 6   y�5�      �� $	 CityCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 5�      ��5�      �� $	 CityErrorText�$
 TwoWay=�$ PropertyChanged=�y�5�      �� $	 CityErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5�      �� 5�      #��[ 6   5�      �� 5�      $ Vertical=�6   .+�5�      �� $��State��6   #��[ 5�      $E ?Up to 50 characters. Where necessary, please use abbreviations.��5�      �� 5�      $ 
Horizontal=�6   .+�5�      �� -   5        $ StateBox��5�      #��[ 60   ��5�      �� $	 StateBoxText�$
 TwoWay=�$ PropertyChanged=�.��6s    5�        5�      $ TextChanged��5�   !    .   5�   !   �� *StateTextChangedCommand+5�   !   .��5�   3   5�      �� 5�      #��[ 6   y�5�      �� $	 StateCheckVisibility�$
 TwoWay=�$ PropertyChanged=�5     �� 5     #��[ 5     ��5     �� $	 StateErrorText�$
 TwoWay=�$ PropertyChanged=�y�5     �� $	 StateErrorVisibility�$
 TwoWay=�$ PropertyChanged=�5     5     �� 5     $��Register��6   #��[ 6(    5       5	     $ Click��5
  1    .   5	  1   �� *SubmitBtnClickCommand+5
  ?   5     5     5     5     