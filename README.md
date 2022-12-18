
# Snork.Blazor.BlockUI

The BlockUI component lets you simulate synchronous behavior when using Blazor, without locking the browser. When activated, it will prevent user activity with the page until it is deactivated.  BlockUI adds elements to the DOM to give it both the appearance and behavior of blocking user interaction.

Usage is very simple.  First, place an instance in your Razor page, with a reference variable:

    <BlockUI @ref="_myBlockUI"/>
    
    @code {
        BlockUI? _myBlockUI;
    }

## Blocking The Page

To block user activity for the page, execute this code:

    await _myBlockUI.Block();

To block with a custom message, there are two approaches.  You can put the custom message into markup directly, like this:

    <BlockUI @ref="_myBlockUI">
        <Template>
            This is the custom message.
        </Template
    </BlockUI>

The second way to use a custom message is:

    await _myBlockUI.Block(@<text>This is the custom message.</text>);

You can use custom styling with the component, as follows:

    await _myBlockUI.WithCss(i =>
        {
            i.BackgroundColor = "#f00";
            i.Color = "#fff";
        }).WithOverlay(i =>
        {
            i.BackgroundColor = "blue";
        }).Block();

## Unblocking the Page

To unblock the page:

    await _myBlockUI.Unblock();