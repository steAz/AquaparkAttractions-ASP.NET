<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Attractions.aspx.cs" Inherits="attractionsAquaparkServerWWW.Attractions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="LabelBoughtTickets" runat="server" Text="Kupione bilety:" Visible="False"></asp:Label>
          
        </div>
        <div id="divTickets" runat="server"></div>
          <br />
            <asp:Label ID="LabelTypeOfTicket" runat="server" Text="Typ biletu (*)" Visible="False"></asp:Label>
        <asp:RadioButtonList ID="RadioButtonList1" runat="server" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged" Visible="False">
            <asp:ListItem>Ulgowy - 18zł</asp:ListItem>
            <asp:ListItem>Rodzinny na 3 osoby - 50zł</asp:ListItem>
            <asp:ListItem Value="Normalny - 22zł">Normalny - 22zł</asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <asp:Label ID="LabelAllowances" runat="server" Text="Dodatki" Visible="False"></asp:Label>
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" Visible="False">
            <asp:ListItem>Strefa saun - 8zł</asp:ListItem>
            <asp:ListItem>Dostęp do zjeżdżalni - 4zł</asp:ListItem>
            <asp:ListItem>Strefa wypoczynkowa - 8zł</asp:ListItem>
        </asp:CheckBoxList>
        <p>
            <asp:Label ID="LabelTime" runat="server" Text="Czas korzystania z aquaparku (*)" Visible="False"></asp:Label>
            <asp:RadioButtonList ID="RadioButtonList2" runat="server" OnSelectedIndexChanged="RadioButtonList2_SelectedIndexChanged" Visible="False">
                <asp:ListItem>1 godzina</asp:ListItem>
                <asp:ListItem>2 godziny</asp:ListItem>
                <asp:ListItem>3 godziny</asp:ListItem>
                <asp:ListItem>do końca dnia</asp:ListItem>
            </asp:RadioButtonList>
        </p>
        <p>
            <asp:Button ID="ButtonPrice" runat="server" Text="Przelicz cenę" OnClick="CountPrice_Click" Visible="False"/>
        </p>
        <p>
            <asp:Label ID="LabelPriceInfo" runat="server" Visible="False"></asp:Label>
            <asp:Label ID="LabelPrice" runat="server" Visible="False"></asp:Label>
        </p>
        <p>
            <asp:Button ID="TicketBuyButton" runat="server" Text="Kup bilet" OnClick="BuyTicket_Click" Visible="False"/>
        </p>
    &nbsp;<asp:Label ID="LabelLogin" runat="server" Text="login"></asp:Label>
        :
        <asp:TextBox ID="TextBoxLogin" runat="server"></asp:TextBox>
&nbsp;
        <asp:Label ID="LabelPass" runat="server" Text="hasło:"></asp:Label>
&nbsp;<asp:TextBox ID="TextBoxPass" runat="server" TextMode="Password"></asp:TextBox>
&nbsp;
        <asp:Button ID="ButtonAuth" runat="server" Text="Zaloguj" OnClick="Auth_Click"/>
&nbsp;
        <asp:Button ID="ButtonRegister" runat="server" Text="Zarejestruj" OnClick="Register_Click"/>
    </form>
</body>
</html>
