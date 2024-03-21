use crate::{
    components::{
        comment::Comment, comment_add::AddComment, markdown::Markdown, post_delete::DeletePost,
        post_edit::EditPost, vote::Vote,
    },
    CommentModel, PostModel,
};
use chrono::{Local, TimeZone};
use std::ops::Deref;
use stylist::Style;
use yew::{function_component, html, use_effect_with, use_state, Callback, Html, Properties};

const STYLE: &str = include_str!("styles/post.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub post: PostModel,
    pub post_number: i32,
    pub on_post_delete: Callback<u32>,
}

#[function_component(Post)]
pub fn post(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let show_comments = use_state(|| false);
    let comments = use_state(|| props.post.comments.clone());
    let description = use_state(|| props.post.description.clone());

    let post_comments = props.post.comments.clone();
    let comments_clone = comments.clone();
    use_effect_with(props.post.comments.clone(), move |_| {
        comments_clone.set(post_comments.clone());
    });

    let comments_clone = comments.clone();
    let on_comment_add = {
        Callback::from(move |comment: CommentModel| {
            let mut new_comments = comments_clone.deref().clone();
            new_comments.insert(0, comment);
            comments_clone.set(new_comments);
        })
    };

    let comments_clone = comments.clone();
    let on_comment_delete = {
        Callback::from(move |id| {
            let mut new_comments = comments_clone.deref().clone();
            if let Some(idx) = new_comments.iter().position(|c| c.id == id) {
                new_comments.remove(idx);
                comments_clone.set(new_comments);
            }
        })
    };

    let on_post_edit = {
        let description = description.clone();
        Callback::from(move |value: String| {
            description.set(value);
        })
    };

    let show_comments_btn = {
        let show_comments = show_comments.clone();
        Callback::from(move |_| {
            show_comments.set(!*show_comments);
        })
    };

    html! {
        <div class={style}>
            <div class="post" id={format!("post{}", props.post.id)}>
                // post info
                <div class="post-info">
                    <span>{"Log "}{props.post_number}</span>
                    <DeletePost id={props.post.id} on_post_delete={&props.on_post_delete}/>
                    <EditPost id={props.post.id} description={props.post.description.clone()} on_post_edit={on_post_edit}/>
                    <span>{Local.from_utc_datetime(&props.post.date).format("%x").to_string()}</span>
                </div>

                // images
                <div>
                    {for props.post.imgs.iter().map(|img| {
                        html! {<img src={img.url.clone()} alt={"dev pic"}/>}
                    })}
                </div>

                // like / dislike
                <Vote up_votes={props.post.up_votes.len()} down_votes={props.post.down_votes.len()} post_id={props.post.id}/>

                // description
                <div class="description">
                    <Markdown content={description.deref().clone()}/>
                </div>

                // comments
                <AddComment post_id={props.post.id} on_comment_add={&on_comment_add}/>
                <div>
                    {for comments.iter().map(|c| {
                        html! {<Comment comment={c.clone()} on_comment_delete={&on_comment_delete} show_comment={*show_comments} />}
                    })}

                    if comments.len() > 5 {
                        <button class="show-all-comments-btn" onclick={show_comments_btn}>
                            {if *show_comments { "Hide Comments" } else { "Show All Comments" }}
                        </button>
                    }
                </div>
            </div>
        </div>
    }
}
