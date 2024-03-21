use std::ops::Deref;

use crate::{
    components::{comment_delete::DeleteComment, comment_edit::EditComment},
    store::Store,
    CommentModel,
};
use chrono::{Local, TimeZone};
use stylist::Style;
use yew::{
    classes, function_component, html, use_effect_with, use_state, Callback, Html, Properties,
};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/comment.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub comment: CommentModel,
    pub on_comment_delete: Callback<u32>,
    pub show_comment: bool,
}

#[function_component(Comment)]
pub fn comment(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let is_editing = use_state(|| false);
    let content = use_state(|| props.comment.content.clone());
    let store = use_store_value::<Store>();
    let date = Local
        .from_utc_datetime(&props.comment.date)
        .format("%x %I:%M %p");

    let is_editing_clone = is_editing.clone();
    let on_is_editing = move |value: bool| {
        is_editing_clone.set(value);
    };

    let content_clone = content.clone();
    let on_edit_save = move |value: String| {
        content_clone.set(value);
    };

    let on_comment_delete_clone = props.on_comment_delete.clone();
    let content_clone = content.clone();
    let comment_clone = props.comment.clone();
    use_effect_with(on_comment_delete_clone, move |_| {
        content_clone.set(comment_clone.content); // dont know why, but I need to have this here in order for the comments to render correctly AFTER one is deleted
    });

    html! {
        <div class={classes!(style, if !props.show_comment { "hide-comment" } else { "" })}>
            <div class="comment">
                <div class="comment-info">
                    // username
                    <span>{&props.comment.username}</span>

                    // edit comment
                    if !*is_editing && props.comment.username == store.username {
                        <EditComment id={props.comment.id}
                            content={content.deref().clone()}
                            is_editing={*is_editing}
                            on_is_editing={on_is_editing.clone()}
                            on_edit_save={on_edit_save.clone()}/>
                    }

                    // delete comment
                    if props.comment.username == store.username || store.admin {
                        <DeleteComment id={props.comment.id.clone()} on_comment_delete={&props.on_comment_delete}/>
                    }

                    // date / time
                    <div class={"date"}>
                        <span>{date.to_string()}</span>
                    </div>
                </div>

                // edit comment
                if *is_editing && props.comment.username == store.username {
                    <EditComment id={props.comment.id}
                        content={props.comment.content.clone()}
                        is_editing={*is_editing}
                        on_is_editing={on_is_editing}
                        on_edit_save={on_edit_save}/>
                }

                // content
                if !*is_editing {
                    <p>{content.deref()}</p>
                }
            </div>
        </div>
    }
}
