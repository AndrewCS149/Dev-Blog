use serde::Deserialize;
use yew::html::Properties;
use yew::prelude::*;

use crate::components::add_comment::AddComment;
use crate::components::comment::*;

#[derive(Deserialize, Clone, PartialEq, Properties)]
pub struct PostProps {
    pub id: i32,
    pub updateNum: String,
    pub description: String,
    pub date: String,
    pub imgURL: String,
    pub comments: Vec<CommentProps>,
}

impl Default for PostProps {
    fn default() -> Self {
        Self {
            id: 0,
            updateNum: "".to_string(),
            description: "".to_string(),
            date: "".to_string(),
            imgURL: "".to_string(),
            comments: Vec::new(),
        }
    }
}

#[function_component]
pub fn Post(props: &PostProps) -> Html {
    let comments = use_state(|| props.comments.to_owned());

    html! {
        <div>
            <div>
                <span>{&props.updateNum}</span>
                <span>{&props.date}</span>
            </div>

            <img src={props.imgURL.clone()}/>
            <p>{&props.description}</p>

            <div>
                {for props.comments.iter().map(|comment| html! {
                    <Comment
                        id={comment.id.clone()}
                        content={comment.content.clone()}
                        date={comment.date.clone()}
                        userName={comment.userName.clone()}
                    />
                })}
            </div>
            <AddComment />
        </div>
    }
}
